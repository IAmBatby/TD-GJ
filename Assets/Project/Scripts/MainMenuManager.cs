using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : GlobalManager
{
    public static new MainMenuManager Instance => SingletonManager.GetSingleton<MainMenuManager>(typeof(MainMenuManager));

    [SerializeField] private Transform mainMenuParent;
    [SerializeField] private Camera mainMenuCamera;
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private GameObject levelSelectMenu;
    [SerializeField] private GameObject mainOptionsMenu;
    [SerializeField] private GameObject levelSceneParent;

    [SerializeField] private ScriptableLevel levelToLoad;

    [SerializeField] private List<ScriptableLevel> allLevelsList = new List<ScriptableLevel>();
    [SerializeField] private LevelSelectElement levelSelectPrefab;
    [SerializeField] private Transform levelSelectStartingPosition;
    [SerializeField] private Vector3 levelSelectSpawnOffset;

    protected override void Awake()
    {
        base.Awake();
        creditsMenu.gameObject.SetActive(false);
        InitializeLevelSelect();
        levelSelectMenu.gameObject.SetActive(false);
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        SceneManager.SetActiveScene(scene);
    }

    private void OnSceneUnloaded(Scene scene)
    {
        mainMenuParent.gameObject.SetActive(true);
        mainMenuCamera.gameObject.SetActive(true);
        levelSceneParent.gameObject.SetActive(true);
    }

    public void LoadGame(ScriptableLevel level)
    {
        mainMenuParent.gameObject.SetActive(false);
        mainMenuCamera.gameObject.SetActive(false);
        levelSceneParent.gameObject.SetActive(false);
        SceneManager.LoadScene(level.SceneName, LoadSceneMode.Additive);
    }

    public void ToggleLevelSelect()
    {
        levelSelectMenu.gameObject.SetActive(!levelSelectMenu.gameObject.activeSelf);
    }

    public void ToggleMainOptionsMenu()
    {
        mainOptionsMenu.gameObject.SetActive(!mainOptionsMenu.gameObject.activeSelf);
    }

    public void ToggleCredits()
    {
        creditsMenu.gameObject.SetActive(!creditsMenu.gameObject.activeSelf);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void InitializeLevelSelect()
    {
        foreach (ScriptableLevel level in allLevelsList)
        {
            LevelSelectElement spawnedElement = GameObject.Instantiate(levelSelectPrefab, levelSelectStartingPosition);
            spawnedElement.transform.position = levelSelectStartingPosition.position;
            spawnedElement.transform.position += (levelSelectSpawnOffset * allLevelsList.IndexOf(level));
            spawnedElement.Initialize(level);
        }

        //The back button because I'm awful
        LevelSelectElement backElement = GameObject.Instantiate(levelSelectPrefab, levelSelectStartingPosition);
        backElement.transform.position = levelSelectStartingPosition.position;
        backElement.transform.position += (levelSelectSpawnOffset * allLevelsList.Count);
        backElement.Initialize(null);
    }
}
