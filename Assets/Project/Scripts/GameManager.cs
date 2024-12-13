using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : GlobalManager
{
    public static new GameManager Instance => SingletonManager.GetSingleton<GameManager>(typeof(GameManager));

    public static PlayerController Player { get; private set; }


    [SerializeField] private float globalSpawnEnemyCooldown;



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

    public List<EnemyAI> AllSpawnedEnemies = new List<EnemyAI>();
    public List<IHittable> AllSpawnedHittables = new List<IHittable>();

    private List<EnemySpawnTarget> AllSpawnTargets;
    private List<EnemyPathTarget> AllPathTargets;

    private Timer ActiveWaveTimer;

    [SerializeField] private List<ScriptableEnemy> RequestedEnemiesToSpawn = new List<ScriptableEnemy>();
    private Timer enemySpawnTimer;


    [SerializeField] private int maxHealth;

    private int currentHealth;

    public int Health { get => currentHealth; set => currentHealth = value; }

    private Dictionary<float, List<ScriptableEnemy>> currentWaveSpawnDict;

    public List<HealthController> AllHealthControllers = new List<HealthController>();


    public ExtendedEvent OnGameStart = new ExtendedEvent();
    public ExtendedEvent<bool> OnGameEnd = new ExtendedEvent<bool>();

    public ExtendedEvent<EnemyAI> OnEnemySpawned = new ExtendedEvent<EnemyAI>();
    public ExtendedEvent OnNewWave = new ExtendedEvent<EnemyAI>();
    public ExtendedEvent OnWaveFinished = new ExtendedEvent();
    public ExtendedEvent OnIntermissionStart = new ExtendedEvent();
    public ExtendedEvent<EnemyAI> OnEnemyKilled = new ExtendedEvent<EnemyAI>();


    private Timer intermissionTimer;
    public float IntermissionLength => DefaultLevel.IntermissionStandardTime + (ActiveWaves.ActiveSelection.WaveLength / DefaultLevel.IntermissionWaveMultiplier);
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
        Player = GameObject.FindObjectOfType<PlayerController>();
    }

    private void Start()
    {
        ResetGame();
        StartNewLevel(DefaultLevel);
    }

    private void ResetGame()
    {
        for (int i = 0; i < AllSpawnedEnemies.Count; i++)
            RemoveEnemy(AllSpawnedEnemies[i]);
        Time.timeScale = 1.0f;
        ChangeGameState(GameState.Playing);
        currentWaveSpawnDict = null;
        enemySpawnTimer = null;
        ActiveWaveTimer = null;
        CurrentLevel = null;
        HasGameEnded = false;
        isFirstWave = true;
        RequestedEnemiesToSpawn = new List<ScriptableEnemy>();
        AllSpawnedEnemies = new List<EnemyAI>();
        AllSpawnedHittables = new List<IHittable>();
        AllSpawnTargets = GameObject.FindObjectsOfType<EnemySpawnTarget>().ToList();
        AllPathTargets = GameObject.FindObjectsOfType<EnemyPathTarget>().ToList();
    }

    private void StartNewLevel(ScriptableLevel level)
    {
        navMeshSurface.BuildNavMesh();
        currentHealth = maxHealth;
        CurrentLevel = ScriptableObject.Instantiate(level);
        ActiveWaves = new SelectableCollection<WaveInfo>(level.Waves);
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
            SceneManager.LoadScene(DefaultLevel.SceneName);
            ResetGame();
            StartNewLevel(DefaultLevel);
        }
    }

    private void LateUpdate()
    {
        RefreshEnemyPriorities();
    }

    public void ModifyHealth(int newValue)
    {
        currentHealth += newValue;
        if (currentHealth <= 0)
            EndGame(didWin: false);

        if (currentHealth < 0)
            currentHealth = 0;
    }

    private void EndGame(bool didWin)
    {
        if (HasGameEnded) return;
        HasGameEnded = true;
        directionalLight.intensity = 0.1f;
        ChangeGameState(GameState.Paused);
        Time.timeScale = 0.3f;

        if (didWin)
            AudioManager.PlayAudio(onGameWonPreset, primarySource);
        else
            AudioManager.PlayAudio(onGameLostPreset, primarySource);
        OnGameEnd.Invoke(didWin);
    }

    public static void EnemyReachedTarget(EnemyAI enemy)
    {
        Debug.Log("Enemy Reached Target!");
        AudioManager.PlayAudio(Instance.onDamageTakenPreset, Instance.primarySource);
        Instance.ModifyHealth(-enemy.Data.Damage);
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

        EnemyAI spawnedEnemy = enemy.SpawnEnemy(randomSpawn, randomTarget);

        enemySpawnTimer = new Timer();
        enemySpawnTimer.onTimerEnd.AddListener(OnEnemySpawnCooldownFinished);
        enemySpawnTimer.StartTimer(this, globalSpawnEnemyCooldown);

        if (spawnedEnemy.TryGetComponent(out IHittable hittable))
            AllSpawnedHittables.Add(hittable);
        AllSpawnedEnemies.Add(spawnedEnemy);

        OnEnemySpawned.Invoke(spawnedEnemy);

    }

    private void RefreshEnemyPriorities()
    {
        List<EnemyAI> sortedEnemies = AllSpawnedEnemies.OrderBy(e => e.Agent.remainingDistance).Reverse().ToList();

        foreach (EnemyAI enemy in sortedEnemies)
            enemy.Agent.avoidancePriority = sortedEnemies.IndexOf(enemy);
    }

    private void OnEnemySpawnCooldownFinished()
    {
        //This shit is cooked
        if (RequestedEnemiesToSpawn.Count > 0)
            RequestedEnemiesToSpawn.Remove(RequestedEnemiesToSpawn.First());
        if (RequestedEnemiesToSpawn.Count > 0)
            SpawnNewEnemy(RequestedEnemiesToSpawn.First());
    }

    public static void RemoveEnemy(EnemyAI enemy)
    {
        if (enemy == null)
        {
            Debug.LogError("Null Enemy Passed Into Remove Enemy");
            return;
        }
        enemy.gameObject.SetActive(false);
        Instance.OnEnemyKilled.Invoke(enemy);
        Instance.AllSpawnedEnemies.Remove(enemy);
        GameObject.Destroy(enemy.gameObject);

        if (Instance.AllSpawnedEnemies.Count == 0)
        {
            Instance.HaveWaveEnemiesBeenRemoved = true;
            Instance.CheckWaveStatus();
        }
    }
}
