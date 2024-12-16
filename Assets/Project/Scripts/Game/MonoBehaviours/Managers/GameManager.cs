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

    public IHighlightable HighlightedObject { get; private set; }

    public ScriptableLevel Level => GlobalData.ActiveLevel;

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

    public static ExtendedEvent OnGameManagerEnable = new ExtendedEvent();
    public static ExtendedEvent OnGameManagerAwake = new ExtendedEvent();
    public static ExtendedEvent OnGameManagerStart = new ExtendedEvent();
    public static ExtendedEvent OnGameStart = new ExtendedEvent();
    public static ExtendedEvent<bool> OnGameEnd = new ExtendedEvent<bool>();
    public static ExtendedEvent<EnemyBehaviour> OnEnemySpawned = new ExtendedEvent<EnemyBehaviour>();
    public static ExtendedEvent OnNewWave = new ExtendedEvent<EnemyBehaviour>();
    public static ExtendedEvent OnWaveFinished = new ExtendedEvent();
    public static ExtendedEvent OnIntermissionStart = new ExtendedEvent();
    public static ExtendedEvent<EnemyBehaviour> OnEnemyKilled = new ExtendedEvent<EnemyBehaviour>();
    public static ExtendedEvent<IHighlightable> OnHighlightChanged = new ExtendedEvent<IHighlightable>();


    private Timer intermissionTimer;
    public float IntermissionLength => Level.IntermissionStandardTime + (Level.IntermissionWaveMultiplier * ActiveWaves.ActiveSelection.WaveLength);
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
        StartNewLevel(Level);

        OnGameManagerEnable.Invoke();
        OnGameManagerAwake.Invoke();
        OnGameManagerStart.Invoke();
    }

    private void ResetGame()
    {
        foreach (EnemyBehaviour enemy in ContentManager.GetBehaviours<EnemyBehaviour>())
            UnregisterContentBehaviour(enemy, true);
        Time.timeScale = 1.0f;
        ChangeGameState(GameState.Playing);
        currentHealth = maxHealth;
        currentCurrency = Level.StartingCurrency;
        currentWaveSpawnDict = null;
        enemySpawnTimer = null;
        ActiveWaveTimer = null;
        intermissionTimer = null;
        CurrentLevel = null;
        HasGameEnded = false;
        isFirstWave = true;
        RequestedEnemiesToSpawn = new List<ScriptableEnemy>();
        AllSpawnTargets = GameObject.FindObjectsOfType<EnemySpawnTarget>().ToList();
        AllPathTargets = GameObject.FindObjectsOfType<EnemyPathTarget>().ToList();
        CurrentLevel = ScriptableObject.Instantiate(Level);
        ActiveWaves = new SelectableCollection<WaveInfo>(Level.Waves);
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
            SceneManager.UnloadSceneAsync(Level.SceneName);
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
        if (HighlightedObject != null && HighlightedObject.GetCursor() != null)
            newCursor = HighlightedObject.GetCursor();
        else
            newCursor = defaultCursor;

        Cursor.SetCursor(newCursor, Vector2.zero, CursorMode.Auto);
    }

    public void OnContentBehaviourMousedEnter(IHighlightable behaviour)
    {
        IHighlightable previousBehaviour = HighlightedObject;
        HighlightedObject = behaviour;
        if (previousBehaviour != HighlightedObject)
            OnHighlightChanged.Invoke(HighlightedObject);
    }

    public void OnContentBehaviourMousedExit(IHighlightable behaviour)
    {
        IHighlightable previousBehaviour = HighlightedObject;
        HighlightedObject = null;
        if (previousBehaviour != HighlightedObject)
            OnHighlightChanged.Invoke(HighlightedObject);
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

        OnEnemySpawned.Invoke(spawnedEnemy);

    }

    private void RefreshEnemyPriorities()
    {
        List<EnemyBehaviour> sortedEnemies = ContentManager.GetBehaviours<EnemyBehaviour>().OrderBy(e => e.RemainingDestinationDistance).Reverse().ToList();

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

        if (enemy.Health == 0 && enemy.EnemyData.ItemDrop != null)
        {
            if (Random.Range(0,100) < enemy.EnemyData.ItemDropRate)
            {
                ItemBehaviour itemDrop = enemy.EnemyData.ItemDrop.SpawnPrefab() as ItemBehaviour;
                itemDrop.transform.position = enemy.transform.position;
            }
        }

        UnregisterContentBehaviour(enemy, true);
        OnEnemyKilled.Invoke(enemy);

        if (ContentManager.GetBehaviours<EnemyBehaviour>().Count == 0)
        {
            Instance.HaveWaveEnemiesBeenRemoved = true;
            Instance.CheckWaveStatus();
        }
    }

    public static void RegisterNewContentBehaviour(ContentBehaviour contentBehaviour)
    {
        contentBehaviour.RegisterBehaviour();
        if (contentBehaviour is PlayerBehaviour player)
            Player = player;
    }

    public static void UnregisterContentBehaviour(ContentBehaviour contentBehaviour, bool destroyOnUnregistration)
    {
        contentBehaviour.UnregisterBehaviour(destroyOnUnregistration);
    }
}
