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
    [SerializeField] private ScriptableLevel CurrentLevel;

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


    public ExtendedEvent OnGameStart = new ExtendedEvent();
    public ExtendedEvent<bool> OnGameEnd = new ExtendedEvent<bool>();

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
        foreach (EnemyAI enemy in AllSpawnedEnemies)
            if (enemy != null)
                GameObject.Destroy(enemy.gameObject);
        Time.timeScale = 1.0f;
        ChangeGameState(GameState.Playing);
        currentWaveSpawnDict = null;
        enemySpawnTimer = null;
        ActiveWaveTimer = null;
        CurrentLevel = null;
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
        StartNextWave();
    }

    private void TryProgressToNextWave()
    {
        if (ActiveWaves.ActiveIndex == ActiveWaves.Collection.Count - 1)
        {
            EndGame(didWin: true);
        }
        else
        {
            ActiveWaves.SelectForward();
            StartNextWave();
        }
    }

    private void StartNextWave()
    {
        Debug.Log("Starting Wave #" + ActiveWaves.ActiveIndex);
        ActiveWaveTimer = new Timer();
        ActiveWaveTimer.onTimerEnd.AddListener(TryProgressToNextWave);
        ActiveWaveTimer.StartTimer(this, ActiveWaves.ActiveSelection.WaveLength);

        currentWaveSpawnDict = ActiveWaves.ActiveSelection.GetEnemyDict();
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            ResetGame();
            StartNewLevel(DefaultLevel);
        }
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
        ChangeGameState(GameState.Paused);
        Time.timeScale = 0.3f;
        OnGameEnd.Invoke(didWin);
    }

    public static void EnemyReachedTarget(EnemyAI enemy)
    {
        Debug.Log("Enemy Reached Target!");
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

        EnemyAI spawnedEnemy = enemy.SpawnEnemy(randomSpawn, randomTarget);

        enemySpawnTimer = new Timer();
        enemySpawnTimer.onTimerEnd.AddListener(OnEnemySpawnCooldownFinished);
        enemySpawnTimer.StartTimer(this, globalSpawnEnemyCooldown);

        if (spawnedEnemy.TryGetComponent(out IHittable hittable))
            AllSpawnedHittables.Add(hittable);
        AllSpawnedEnemies.Add(spawnedEnemy);

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
        enemy.gameObject.SetActive(false);
        Instance.AllSpawnedEnemies.Remove(enemy);
        GameObject.Destroy(enemy.gameObject);
    }
}
