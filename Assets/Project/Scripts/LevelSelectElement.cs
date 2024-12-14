using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelSelectElement : MonoBehaviour
{
    public TextMeshProUGUI levelName;
    [SerializeField] private ScriptableLevel level;

    private void OnEnable()
    {
        //levelName.enabled = false;
    }

    public void Initialize(ScriptableLevel newLevel, int index)
    {
        level = newLevel;
        levelName.enabled = true;
        if (level != null)
            levelName.SetText("#0" + (index + 1) + ": " + level.DisplayName.ToUpper());
        else
            levelName.SetText("Back".ToUpper());
    }

    public void LoadLevel()
    {
        if (level != null)
            MainMenuManager.Instance.LoadGame(level);
        else
        {
            MainMenuManager.Instance.ToggleLevelSelect();
            MainMenuManager.Instance.ToggleMainOptionsMenu();
        }
    }
}
