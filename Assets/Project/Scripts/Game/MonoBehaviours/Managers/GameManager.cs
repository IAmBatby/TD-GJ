using IterationToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : GlobalManager
{
    public static new GameManager Instance => SingletonManager.GetSingleton<GameManager>(typeof(GameManager));

    public static PlayerBehaviour Player { get; private set; }


    [SerializeField] private float globalSpawnEnemyCooldown;

    [SerializeField] private Texture2D defaultCursor;
    private Texture2D activeCursor;
    private Texture2D lastSetCursor;

    public ContentBehaviour HighlightedBehaviour { get; private set; }

    [field: SerializeField] public ScriptableLevel DefaultLevel { get; private set; }

    [SerializeField] private NavMeshSurface navMeshSurface;
    [SerializeField] private Light directionalLight;
    [SerializeField] private ScriptableLevel CurrentLevel;
    [SerializeField] private AudioSource primarySource;
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioPreset onGameStartPreset;
    [SerializeField] private AudioPreset onWaveStartPreset;
    [SerializeField] private AudioPreset onWaveEndPreset;
    [SerializeField] private AudioPreset onGameWonPreset;
    [SerializeField] private AudioPreset onGameLostPreset;
    [SerializeField] private AudioPreset onDamageTakenPreset;
    [SerializeField] private AudioPreset ambiencePreset;
    [SerializeField] private AudioPreset onCurrencyGainedPreset;

    [SerializeField] private SelectableCollection<WaveInfo> ActiveWaves;

    public int CurrentWaveCount => ActiveWaves.ActiveIndex;
    public int TotalWaveCount => ActiveWaves.Collection.Count;

    public float CurrentWaveTime
    {
        get
        {
            if (ActiveWaveTimer != null)
                return (ActiveWaveTimer.Progress);
            return (0);                 
        }
    }

    public float TotalWaveTime
    {
        get
        {
            if (ActiveWaves != null && ActiveWaves.ActiveSelection != null)
                return (ActiveWaves.ActiveSelection.WaveLength);
            return (0);
        }
    }

    private Dictionary<ScriptableContent, List<ContentBehaviour>> allRegisteredBehavioursDict = new Dictionary<ScriptableContent, List<ContentBehaviour>>();

    public List<IHittable> AllSpawnedHittables = new List<IHittable>();

    private List<EnemySpawnTarget> AllSpawnTargets;
    private List<EnemyPathTarget> AllPathTargets;

    private Timer ActiveWaveTimer;

    [SerializeField] private List<ScriptableEnemy> RequestedEnemiesToSpawn = new List<ScriptableEnemy>();
    private Timer enemySpawnTimer;


    [SerializeField] private int maxHealth;

    private int currentHealth;

    public int Health { get => currentHealth; set => currentHealth = value; }

    private int currentCurrency;

    public int Currency { get => currentCurrency; set => ModifyCurrency(value); }

    private Dictionary<float, List<ScriptableEnemy>> currentWaveSpawnDict;

    public List<HealthController> AllHealthControllers = new List<HealthController>();


    public ExtendedEvent OnGameStart = new ExtendedEvent();
    public ExtendedEvent<bool> OnGameEnd = new ExtendedEvent<bool>();

    public ExtendedEvent<EnemyBehaviour> OnEnemySpawned = new ExtendedEvent<EnemyBehaviour>();
    public ExtendedEvent OnNewWave = new ExtendedEvent<EnemyBehaviour>();
    public ExtendedEvent OnWaveFinished = new ExtendedEvent();
    public ExtendedEvent OnIntermissionStart = new ExtendedEvent();
    public ExtendedEvent<EnemyBehaviour> OnEnemyKilled = new ExtendedEvent<EnemyBehaviour>();


    private Timer intermissionTimer;
    public float IntermissionLength => DefaultLevel.IntermissionStandardTime + (DefaultLevel.IntermissionWaveMultiplier * ActiveWaves.ActiveSelection.WaveLength);
    public float IntermissionProgress
    {
        get
        {
            if (intermissionTimer == null)
                return (0f);
            return (intermissionTimer.TimeElapsed);
        }
    }
    [field: SerializeField] public bool IsInIntermission { get; private set; }

    [field: SerializeField] public bool HasWaveTimeFinished { get; private set; }
    [field: SerializeField] public bool HaveWaveEnemiesBeenRemoved { get; private set; }

    [field: SerializeField] public bool HasGameEnded { get; private set; }

    private bool isFirstWave;
    protected override void Awake()
    {
        base.Awake();
        Player = GameObject.FindObjectOfType<PlayerBehaviour>();
    }

    private void Start()
    {
        ResetGame();
        StartNewLevel(DefaultLevel);
    }

    private void ResetGame()
    {
        foreach (EnemyBehaviour enemy in GetContentBehaviours<EnemyBehaviour>())
            UnregisterContentBehaviour(enemy, true);
        Time.timeScale = 1.0f;
        ChangeGameState(GameState.Playing);
        currentHealth = maxHealth;
        currentCurrency = DefaultLevel.StartingCurrency;
        currentWaveSpawnDict = null;
        enemySpawnTimer = null;
        ActiveWaveTimer = null;
        intermissionTimer = null;
        CurrentLevel = null;
        HasGameEnded = false;
        isFirstWave = true;
        RequestedEnemiesToSpawn = new List<ScriptableEnemy>();
        AllSpawnedHittables = new List<IHittable>();
        AllSpawnTargets = GameObject.FindObjectsOfType<EnemySpawnTarget>().ToList();
        AllPathTargets = GameObject.FindObjectsOfType<EnemyPathTarget>().ToList();
        CurrentLevel = ScriptableObject.Instantiate(DefaultLevel);
        ActiveWaves = new SelectableCollection<WaveInfo>(DefaultLevel.Waves);
        UIManager.Instance.InitializeUI();
    }

    private void StartNewLevel(ScriptableLevel level)
    {
        navMeshSurface.BuildNavMesh();
        OnGameStart.Invoke();
        AudioManager.PlayAudio(ambiencePreset, ambienceSource);
        AudioManager.PlayAudio(onGameStartPreset, primarySource);
        //StartNextWave();
        StartIntermission();
    }

    private void TryProgressToNextWave()
    {
        if (ActiveWaves.ActiveIndex == ActiveWaves.Collection.Count - 1)
        {
            EndGame(didWin: true);
        }
        else
        {
            if (isFirstWave == true)
                isFirstWave = false;
            else
                ActiveWaves.SelectForward();
            StartNextWave();
        }
    }

    private void StartNextWave()
    {
        Debug.Log("Starting Wave #" + ActiveWaves.ActiveIndex);
        HasWaveTimeFinished = false;
        IsInIntermission = false;


        ActiveWaveTimer = new Timer();
        ActiveWaveTimer.onTimerEnd.AddListener(OnWaveTimeFinished);
        ActiveWaveTimer.StartTimer(this, ActiveWaves.ActiveSelection.WaveLength);

        currentWaveSpawnDict = ActiveWaves.ActiveSelection.GetEnemyDict();

        AudioManager.PlayAudio(onWaveStartPreset, primarySource);
        OnNewWave.Invoke();
    }

    private void OnWaveTimeFinished()
    {
        HasWaveTimeFinished = true;
        CheckWaveStatus();
    }

    private void CheckWaveStatus()
    {
        if (HasGameEnded) return;
        if (HasWaveTimeFinished && HaveWaveEnemiesBeenRemoved)
        {
            AudioManager.PlayAudio(onWaveEndPreset, primarySource);
            OnWaveFinished.Invoke();
            StartIntermission();
        }
    }

    private void StartIntermission()
    {
        IsInIntermission = true;
        intermissionTimer = new Timer();
        intermissionTimer.onTimerEnd.AddListener(TryProgressToNextWave);
        intermissionTimer.StartTimer(this, IntermissionLength);
        OnIntermissionStart.Invoke();
    }

    private void Update()
    {
        if (currentWaveSpawnDict != null && currentWaveSpawnDict.Count > 0)
        {
            float time = currentWaveSpawnDict.First().Key;
            List<ScriptableEnemy> enemy = currentWaveSpawnDict.First().Value;
            if (time < CurrentWaveTime)
            {
                foreach (ScriptableEnemy enemyToSpawn in enemy)
                    RequestNewEnemySpawn(enemyToSpawn);
                currentWaveSpawnDict.Remove(currentWaveSpawnDict.First().Key);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.UnloadSceneAsync(DefaultLevel.SceneName);
            //ResetGame();
            //StartNewLevel(DefaultLevel);
        }
    }

    private void LateUpdate()
    {
        RefreshEnemyPriorities();
        RefreshCursor();
    }

    private void RefreshCursor()
    {
        Texture2D newCursor = null;
        if (HighlightedBehaviour != null && HighlightedBehaviour.ContentData.Cursor != null)
            newCursor = HighlightedBehaviour.ContentData.Cursor;
        else
            newCursor = defaultCursor;

        Cursor.SetCursor(newCursor, Vector2.zero, CursorMode.Auto);
    }

    public void OnContentBehaviourMousedEnter(ContentBehaviour behaviour)
    {
        HighlightedBehaviour = behaviour;
    }

    public void OnContentBehaviourMousedExit(ContentBehaviour behaviour)
    {
        HighlightedBehaviour = null;
    }

    public void ModifyHealth(int newValue)
    {
        currentHealth += newValue;
        if (currentHealth <= 0)
            EndGame(didWin: false);

        if (currentHealth < 0)
            currentHealth = 0;
    }

    public static void ModifyCurrency(int value)
    {
        Instance.currentCurrency += value;
        if (value > 0)
            AudioManager.PlayAudio(Instance.onCurrencyGainedPreset, Instance.primarySource);
    }

    public void EndGame(bool didWin)
    {
        if (HasGameEnded) return;
        HasGameEnded = true;
        if (directionalLight != null)
            directionalLight.intensity = 0.1f;
        ChangeGameState(GameState.Paused);
        Time.timeScale = 0.3f;

        if (didWin)
            AudioManager.PlayAudio(onGameWonPreset, primarySource);
        else
            AudioManager.PlayAudio(onGameLostPreset, primarySource);
        OnGameEnd.Invoke(didWin);
    }

    public static void EnemyReachedTarget(EnemyBehaviour enemy)
    {
        Debug.Log("Enemy Reached Target!");
        AudioManager.PlayAudio(Instance.onDamageTakenPreset, Instance.primarySource);
        Instance.ModifyHealth(-enemy.EnemyData.Damage);
        RemoveEnemy(enemy);
    }

    private void RequestNewEnemySpawn(ScriptableEnemy enemy)
    {
        RequestedEnemiesToSpawn.Add(enemy);
        if (RequestedEnemiesToSpawn.Count == 1)
            SpawnNewEnemy(enemy);
    }

    private void SpawnNewEnemy(ScriptableEnemy enemy)
    {
        Debug.Log("Spawning Enemy: " + enemy.name);
        EnemySpawnTarget randomSpawn = AllSpawnTargets[Random.Range(0, AllSpawnTargets.Count)];
        EnemyPathTarget randomTarget = AllPathTargets[Random.Range(0, AllPathTargets.Count)];

        HaveWaveEnemiesBeenRemoved = false;

        //EnemyAI spawnedEnemy = enemy.SpawnEnemy(randomSpawn, randomTarget);

        EnemyBehaviour spawnedEnemy = enemy.SpawnPrefab() as EnemyBehaviour;
        spawnedEnemy.transform.position = randomSpawn.transform.position;
        spawnedEnemy.RecieveNewTarget(randomTarget);

        enemySpawnTimer = new Timer();
        enemySpawnTimer.onTimerEnd.AddListener(OnEnemySpawnCooldownFinished);
        enemySpawnTimer.StartTimer(this, globalSpawnEnemyCooldown);

        if (spawnedEnemy.TryGetComponent(out IHittable hittable))
            AllSpawnedHittables.Add(hittable);

        OnEnemySpawned.Invoke(spawnedEnemy);

    }

    private void RefreshEnemyPriorities()
    {
        List<EnemyBehaviour> sortedEnemies = GetContentBehaviours<EnemyBehaviour>().OrderBy(e => e.RemainingDestinationDistance).Reverse().ToList();

        foreach (EnemyBehaviour enemy in sortedEnemies)
            enemy.SetAvoidancePriority(sortedEnemies.IndexOf(enemy));
    }

    private void OnEnemySpawnCooldownFinished()
    {
        //This shit is cooked
        if (RequestedEnemiesToSpawn.Count > 0)
            RequestedEnemiesToSpawn.Remove(RequestedEnemiesToSpawn.First());
        if (RequestedEnemiesToSpawn.Count > 0)
            SpawnNewEnemy(RequestedEnemiesToSpawn.First());
    }

    public static void RemoveEnemy(EnemyBehaviour enemy)
    {
        if (enemy == null)
        {
            Debug.LogError("Null Enemy Passed Into Remove Enemy");
            return;
        }

        UnregisterContentBehaviour(enemy, true);
        Instance.OnEnemyKilled.Invoke(enemy);

        if (GetContentBehaviours<EnemyBehaviour>().Count == 0)
        {
            Instance.HaveWaveEnemiesBeenRemoved = true;
            Instance.CheckWaveStatus();
        }
    }

    public static void RegisterNewContentBehaviour(ContentBehaviour contentBehaviour)
    {
        if (contentBehaviour == null || contentBehaviour.ContentData == null)
        {
            Debug.LogError("Content failed to register!");
            return;
        }

        if (Instance.allRegisteredBehavioursDict.TryGetValue(contentBehaviour.ContentData, out List<ContentBehaviour> behaviours))
        {
            if (behaviours.Contains(contentBehaviour))
            {
                Debug.LogError("Content attempted to register itself while already being registered!");
                return;       
            }
            else
                behaviours.Add(contentBehaviour);
        }
        else
            Instance.allRegisteredBehavioursDict.Add(contentBehaviour.ContentData, new List<ContentBehaviour> { contentBehaviour });

        if (contentBehaviour is PlayerBehaviour playerBehaviour)
            Player = playerBehaviour;
    }

    public static List<T> GetContentBehaviours<T>(ScriptableContent contentData) where T : ContentBehaviour
    {
        if (Instance.allRegisteredBehavioursDict.TryGetValue(contentData, out List<ContentBehaviour> behaviours))
            return (behaviours as List<T>);
        else
            return (new List<T>());         
    }

    public static List<T> GetContentBehaviours<T>() where T : ContentBehaviour
    {
        List<T> returnList = new List<T>();
        foreach (KeyValuePair<ScriptableContent, List<ContentBehaviour>> registeredLists in Instance.allRegisteredBehavioursDict)
            if (registeredLists.Value.First() is T)
                foreach (ContentBehaviour behaviour in registeredLists.Value)
                    returnList.Add(behaviour as T);

        return (returnList);
    }

    public static void UnregisterContentBehaviour(ContentBehaviour contentBehaviour, bool destroyOnUnregistration)
    {
        if (contentBehaviour == null || contentBehaviour.ContentData == null)
        {
            Debug.LogError("Content failed to unregister!");
            return;
        }

        if (Instance.allRegisteredBehavioursDict.TryGetValue(contentBehaviour.ContentData, out List<ContentBehaviour> behaviours))
        {
            if (behaviours.Contains(contentBehaviour))
            {
                if (behaviours.Count == 1)
                    Instance.allRegisteredBehavioursDict.Remove(contentBehaviour.ContentData);
                else
                    behaviours.Remove(contentBehaviour);
            }
            else
                Debug.LogError("Content attempted to unregister without being registered!");
        }
        else
            Debug.LogError("Content attempted to unregister without being registered!");

        if (destroyOnUnregistration == true)
        {
            contentBehaviour.enabled = false;
            contentBehaviour.gameObject.SetActive(false);
            GameObject.Destroy(contentBehaviour.gameObject);
        }
    }
}
