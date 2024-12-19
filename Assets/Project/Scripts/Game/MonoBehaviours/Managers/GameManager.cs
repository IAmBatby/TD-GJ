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
using Logger = IterationToolkit.Logger;

public struct LogCollection
{
    public Logger Main { get; private set; }
    public Logger Behaviours { get; private set; }
    public Logger Wave { get; private set; }
    public Logger Enemies { get; private set; }

    public LogCollection(int maxLines = 0)
    {
        Main = DynamicConsole.AddLogger("Main");
        Behaviours = DynamicConsole.AddLogger("Behaviours");
        Wave = DynamicConsole.AddLogger("Wave");
        Enemies = DynamicConsole.AddLogger("Enemies");
    }
}


public class GameManager : GlobalManager
{
    ////////// Singleton / Global References //////////
    public static new GameManager Instance => SingletonManager.GetSingleton<GameManager>(typeof(GameManager));
    public static PlayerBehaviour Player { get; private set; }
    public ScriptableLevel Level => GlobalData.ActiveLevel;
    [SerializeField] private Texture2D defaultCursor;

    private LogCollection logs;
    public static LogCollection Logs => Instance.logs;

    ////////// Serialized Component References //////////

    [SerializeField] private NavMeshSurface navMeshSurface;
    [SerializeField] private Light directionalLight;

    ////////// Serialized Audio References //////////
    
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

    ////////// ScriptableWave Collection //////////
    [SerializeField] private SelectableCollection<ScriptableWave> ActiveWaves;
    public static ScriptableWave CurrentWave => Instance.Level.WaveManifest.Waves[CurrentWaveCount];
    public static int CurrentWaveCount => Instance.ActiveWaves.ActiveIndex;
    public static int TotalWaveCount => Instance.ActiveWaves.Collection.Count;

    ////////// Current Wave Timers //////////

    private Timer currentWaveTimer;
    public static float CurrentWaveProgress => Instance.currentWaveTimer != null ? Instance.currentWaveTimer.Progress : 0f;
    public static float CurrentWaveLength => Instance.ActiveWaves.ActiveSelection.GetFinalTime();

    private Timer intermissionTimer;
    public static float IntermissionLength => CurrentWave.IntermissionTimeLength;
    public static float IntermissionProgress => Instance.intermissionTimer != null ? Instance.intermissionTimer.TimeElapsed : 0;

    ////////// GameManager Attributes //////////

    [SerializeField] private int maxHealth;
    private int currentHealth;
    public static int Health { get => Instance.currentHealth; set => Instance.currentHealth = value; }

    private int currentCurrency;
    public static int Currency { get => Instance.currentCurrency; set => ModifyCurrency(value); }

    ////////// Enemy Spawning //////////

    private List<EnemySpawnTarget> AllSpawnTargets;
    private List<EnemyPathTarget> AllPathTargets;

    private List<Timer<ScriptableEnemy>> activeSpawnRequests = new List<Timer<ScriptableEnemy>>();

    ////////// IHighlightable Management //////////

    private Texture2D activeCursor;
    private Texture2D lastSetCursor;
    public static IHighlightable HighlightedObject { get; private set; }
    private MaterialCache highlightCache;

    ////////// Bools //////////

    public static bool IsLevelActive => Instance.currentWaveTimer != null || Instance.intermissionTimer != null ? true : false;
    public static bool IsLevelLost => Health > 0 && Player?.Health > 0 ? true : false;
    public static bool IsInActiveWave => Instance.currentWaveTimer != null;

    private bool isConsoleEnabled;

    ////////// Game Events //////////

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

    protected override void Awake()
    {
        base.Awake();
        AllSpawnTargets = GameObject.FindObjectsOfType<EnemySpawnTarget>().ToList();
        AllPathTargets = GameObject.FindObjectsOfType<EnemyPathTarget>().ToList();
        OnHighlightChanged.AddListener(RefreshCursor);
        logs = new LogCollection(10);
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
        foreach (Timer<ScriptableEnemy> activeSpawnTimer in activeSpawnRequests)
            activeSpawnTimer.TryStopTimer();
        activeSpawnRequests.Clear();
        Time.timeScale = 1.0f;
        ChangeGameState(GameState.Playing);
        currentHealth = Level.StartingHealth;
        currentCurrency = Level.StartingCurrency;
        currentWaveTimer = null;
        intermissionTimer = null;
        ActiveWaves = new SelectableCollection<ScriptableWave>(Level.WaveManifest.Waves);
        UIManager.Instance.InitializeUI();
    }

    private void StartNewLevel(ScriptableLevel level)
    {
        navMeshSurface.BuildNavMesh();
        OnGameStart.Invoke();
        AudioManager.PlayAudio(ambiencePreset, ambienceSource);
        AudioManager.PlayAudio(onGameStartPreset, primarySource);
        Logs.Main.LogInfo(("Starting Level: " + Level.DisplayName).ToBold());
        StartIntermission();
    }

    private void TryProgressToNextWave()
    {
        if (ActiveWaves.ActiveIndex == ActiveWaves.Collection.Count - 1)
        {
            EndGame();
        }
        else
        {
            if (CurrentWaveCount > 0)
                ActiveWaves.SelectForward();
            StartNextWave();
        }
    }

    private void SkipAllWaves()
    {
        Debug.Log("Skipping All Waves");
        while (IsLevelActive == true)
            TryProgressToNextWave();
    }

    private void StartNextWave()
    {

        currentWaveTimer = new Timer(this, Level.WaveManifest.GetWaveLength(CurrentWaveCount));

        activeSpawnRequests.Clear();
        foreach ((ScriptableEnemy, float) spawnRequest in CurrentWave.GetEnemySpawnManifest())
            activeSpawnRequests.Add(new Timer<ScriptableEnemy>(this, spawnRequest.Item2, spawnRequest.Item1, SpawnNewEnemy, RemoveLastRequest));

        AudioManager.PlayAudio(onWaveStartPreset, primarySource);
        Logs.Wave.LogInfo("Started Wave #" + CurrentWaveCount);
        OnNewWave.Invoke();
    }

    private void RemoveLastRequest(ScriptableEnemy _)
    {
        if (activeSpawnRequests.Count > 0)
            activeSpawnRequests.RemoveAt(0);
    }

    private void CheckWaveStatus()
    {
        if (IsLevelActive == false) return;
        if (ContentManager.GetBehaviours<EnemyBehaviour>().Count == 0)
        {
            AudioManager.PlayAudio(onWaveEndPreset, primarySource);
            OnWaveFinished.Invoke();
            StartIntermission();
        }
    }

    private void StartIntermission()
    {
        currentWaveTimer = null;
        intermissionTimer = new Timer(this, CurrentWave.IntermissionTimeLength, TryProgressToNextWave);
        OnIntermissionStart.Invoke();
        Logs.Wave.LogInfo("Started Intermission #" + CurrentWaveCount);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            SkipAllWaves();

        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.UnloadSceneAsync(Level.SceneName);

        if (Input.GetKeyDown(KeyCode.C))
            isConsoleEnabled = !isConsoleEnabled;

        if (Input.GetKeyDown(KeyCode.Tab))
            DynamicConsole.ToggleForward();
        if (Input.GetKeyDown(KeyCode.LeftShift))
            DynamicConsole.ToggleBackward();
    }

    private void RefreshCursor(IHighlightable highlightable)
    {
        Cursor.SetCursor(highlightable?.GetCursor() != null ? highlightable?.GetCursor() : defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    public void OnContentBehaviourMousedEnter(IHighlightable behaviour)
    {
        IHighlightable previousBehaviour = HighlightedObject;
        HighlightedObject = behaviour;
        if (previousBehaviour != HighlightedObject)
        {
            OnHighlightChanged.Invoke(HighlightedObject);
            if (previousBehaviour != null)
                previousBehaviour.GetMaterialController().ResetRenderers();
            if (HighlightedObject != null && HighlightedObject.IsHighlightable())
                HighlightedObject.GetMaterialController().ApplyMaterial(GlobalData.Instance.PreviewMaterial, HighlightedObject.GetColor());
        }

    }

    public void OnContentBehaviourMousedExit(IHighlightable behaviour)
    {
        IHighlightable previousBehaviour = HighlightedObject;
        HighlightedObject = null;
        if (previousBehaviour != HighlightedObject)
        {
            OnHighlightChanged.Invoke(HighlightedObject);
            if (previousBehaviour != null)
                previousBehaviour.GetMaterialController().ResetRenderers();
        }
    }

    public void ModifyHealth(int newValue)
    {
        currentHealth = Math.Clamp(currentHealth + newValue, 0, 99999);
        if (currentHealth <= 0)
            EndGame();
    }

    public static void ModifyCurrency(int value)
    {
        Instance.currentCurrency += value;
        if (value > 0)
            AudioManager.PlayAudio(Instance.onCurrencyGainedPreset, Instance.primarySource);
    }

    public void EndGame()
    {
        if (IsLevelActive == false) return;
        currentWaveTimer = null;
        intermissionTimer = null;

        if (directionalLight != null)
            directionalLight.intensity = 0.1f;
        ChangeGameState(GameState.Paused);
        Time.timeScale = 0.3f;

        AudioManager.PlayAudio(IsLevelLost ? onGameLostPreset : onGameWonPreset, primarySource);
        OnGameEnd.Invoke(IsLevelLost);
    }

    public static void EnemyReachedTarget(EnemyBehaviour enemy)
    {
        Logs.Enemies.LogInfo("Enemy: " + enemy.ContentData.GetDisplayName() + " Reached End Target", Color.red);
        AudioManager.PlayAudio(Instance.onDamageTakenPreset, Instance.primarySource);
        Instance.ModifyHealth(-enemy.EnemyData.Damage);
        RemoveEnemy(enemy);
    }

    private void SpawnNewEnemy(ScriptableEnemy enemy)
    {
        EnemySpawnTarget randomSpawn = AllSpawnTargets[Random.Range(0, AllSpawnTargets.Count)];
        EnemyPathTarget randomTarget = AllPathTargets[Random.Range(0, AllPathTargets.Count)];

        EnemyBehaviour spawnedEnemy = enemy.SpawnPrefab() as EnemyBehaviour;
        spawnedEnemy.transform.position = randomSpawn.transform.position;
        spawnedEnemy.RecieveNewTarget(randomTarget);
        Logs.Enemies.LogInfo("Spawned New Enemy: " + enemy.GetDisplayName(), Color.yellow);
        OnEnemySpawned.Invoke(spawnedEnemy);
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

        OnEnemyKilled.Invoke(enemy);
        UnregisterContentBehaviour(enemy, true);
        Instance.CheckWaveStatus();
    }

    public static void RegisterNewContentBehaviour(ContentBehaviour contentBehaviour)
    {
        Logs.Behaviours.LogInfo("Registered New Content: " + contentBehaviour.ContentData.GetDisplayName().ToBold(), contentBehaviour.ContentData.GetDisplayColor());
        contentBehaviour.RegisterBehaviour();
        if (contentBehaviour is PlayerBehaviour player)
            Player = player;
    }

    public static void UnregisterContentBehaviour(ContentBehaviour contentBehaviour, bool destroyOnUnregistration)
    {
        Logs.Behaviours.LogInfo("Unregistered New Content: " + contentBehaviour.ContentData.GetDisplayName().ToBold(), contentBehaviour.ContentData.GetDisplayColor());
        contentBehaviour.UnregisterBehaviour(destroyOnUnregistration);
    }

    private void OnGUI()
    {
        //DynamicConsole.RenderConsole();
        if (isConsoleEnabled)
            DynamicConsole.RenderConsole();
    }
}
