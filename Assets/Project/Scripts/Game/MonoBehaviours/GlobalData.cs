using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalData : MonoBehaviour
{
    static GlobalData _instance;
    public static GlobalData Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GlobalData>();
            return _instance;
        }
    }

    [SerializeField] private ColorManifest colorManifest;
    [SerializeField] private AttributeManifest attributeManifest;
    [SerializeField] private LevelManifest levelManifest;
    [SerializeField] private IconManifest iconManifest;
    [SerializeField] private SkinManifest skinManifest;

    [SerializeField] private GameObject systemsPrefab;


    public static LevelManifest Levels => Instance.levelManifest;
    public static ColorManifest Colors => Instance.colorManifest;
    public static AttributeManifest Attributes => Instance.attributeManifest;
    public static IconManifest Icons => Instance.iconManifest;
    public static SkinManifest Skins => Instance.skinManifest;

    private static Dictionary<string, ScriptableLevel> sceneLevelDict = new Dictionary<string, ScriptableLevel>();

    private static string globalDataSceneName = "GlobalData";

    public static ScriptableLevel ActiveLevel { get; private set; }

    public static string startingScene;

    public static ExtendedEvent<ScriptableLevel> OnLevelUnloaded = new ExtendedEvent<ScriptableLevel>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void LoadGlobalDataScene()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("GlobalData Initializing, Active Scene Is: " + SceneManager.GetActiveScene().name);
        startingScene = SceneManager.GetActiveScene().name;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        SceneManager.LoadScene(globalDataSceneName, LoadSceneMode.Additive);
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("GlobalData Scene Loaded: " + scene.name);
        if (scene.name == globalDataSceneName)
            OnGlobalSceneLoaded(scene);
        else if (sceneLevelDict.TryGetValue(scene.name, out ScriptableLevel value))
            InitializeLevel(scene, value);
            
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            ContentManager.DebugAllContent();
    }

    private static void OnGlobalSceneLoaded(Scene scene)
    {
        DontDestroyOnLoad(Instance.gameObject);
        sceneLevelDict = Levels.GetLevelSceneDict();
        OnSceneLoaded(SceneManager.GetSceneByName(startingScene), LoadSceneMode.Additive);
        UnloadGlobalDataScene(scene);
    }

    private static void InitializeLevel(Scene scene, ScriptableLevel level)
    {
        Debug.Log("Initializing Level: " + level.DisplayName);
        ActiveLevel = level;

        SceneManager.SetActiveScene(scene);

        GameObject spawnedSystems = GameObject.Instantiate(Instance.systemsPrefab);

    }

    private static void OnSceneUnloaded(Scene scene)
    {
        if (ActiveLevel != null && ActiveLevel.SceneName == scene.name)
        {
            OnLevelUnloaded.Invoke(ActiveLevel);    
            ActiveLevel = null;
        }
    }

    private static void UnloadGlobalDataScene(Scene scene)
    {
        SceneManager.UnloadSceneAsync(scene);
    }
}
