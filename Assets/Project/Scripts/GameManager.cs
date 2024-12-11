using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : GlobalManager
{
    public static new GameManager Instance => SingletonManager.GetSingleton<GameManager>(typeof(GameManager));

    [field: SerializeField] public ScriptableLevel DefaultLevel { get; private set; }

    [SerializeField] private ScriptableLevel CurrentLevel;

    [SerializeField] private SelectableCollection<WaveInfo> ActiveWaves;

    public int CurrentWaveCount => ActiveWaves.ActiveIndex;
    public int TotalWaveCount => ActiveWaves.Collection.Count;

    public float CurrentWaveTime => ActiveWaveTimer.Progress;
    public float TotalWaveTime => ActiveWaves.ActiveSelection.WaveLength;

    private List<EnemyAI> AllSpawnedEnemies;

    private List<EnemySpawnTarget> AllSpawnTargets;
    private List<EnemyPathTarget> AllPathTargets;

    private Timer ActiveWaveTimer;


    [SerializeField] private int maxHealth;

    private int currentHealth;

    public int Health { get => currentHealth; set => currentHealth = value; }

    private void Start()
    {
        StartNewLevel(DefaultLevel);
    }

    private void StartNewLevel(ScriptableLevel level)
    {
        currentHealth = maxHealth;
        CurrentLevel = ScriptableObject.Instantiate(level);
        ActiveWaves = new SelectableCollection<WaveInfo>(level.Waves);
        AllSpawnedEnemies = new List<EnemyAI>();
        AllSpawnTargets = GameObject.FindObjectsOfType<EnemySpawnTarget>().ToList();
        AllPathTargets = GameObject.FindObjectsOfType<EnemyPathTarget>().ToList();
        TryProgressToNextWave();
    }

    private void TryProgressToNextWave()
    {
        if (ActiveWaves.ActiveIndex == ActiveWaves.Collection.Count - 1)
        {
            Debug.Log("YOU WINNIE");
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


        //For now
        List<ScriptableEnemy> allEnemies = new List<ScriptableEnemy>();
        foreach (EnemySpawnInfo spawnInfo in ActiveWaves.ActiveSelection.EnemySpawnInfos)
            foreach (ScriptableEnemy enemy in spawnInfo.EnemiesToSpawn)
                allEnemies.Add(enemy);

        for (int i = 0; i < allEnemies.Count; i++)
            SpawnNewEnemy(allEnemies[i]);

    }

    public static void EnemyReachedTarget(EnemyAI enemy)
    {
        Instance.currentHealth -= enemy.Data.Damage;
        enemy.gameObject.SetActive(false);
        GameObject.Instantiate(enemy.gameObject);
    }

    private void SpawnNewEnemy(ScriptableEnemy enemy)
    {
        Debug.Log("Spawning Enemy: " + enemy.name);
        EnemySpawnTarget randomSpawn = AllSpawnTargets[Random.Range(0, AllSpawnTargets.Count)];
        EnemyPathTarget randomTarget = AllPathTargets[Random.Range(0, AllPathTargets.Count)];

        EnemyAI spawnedEnemy = enemy.SpawnEnemy(randomSpawn, randomTarget);
    }
}
