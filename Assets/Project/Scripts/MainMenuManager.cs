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

    [SerializeField] private ScriptableLevel levelToLoad;

    protected override void Awake()
    {
        base.Awake();
        creditsMenu.gameObject.SetActive(false);
    }

    public void LoadGame()
    {
        mainMenuParent.gameObject.SetActive(false);
        mainMenuCamera.gameObject.SetActive(false);
        SceneManager.LoadScene(levelToLoad.SceneName, LoadSceneMode.Additive);
    }

    public void ToggleCredits()
    {
        creditsMenu.gameObject.SetActive(!creditsMenu.gameObject.activeSelf);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
