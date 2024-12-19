using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : GlobalManager
{
    public static new UIManager Instance => SingletonManager.GetSingleton<UIManager>(typeof(UIManager));

    [SerializeField] private TextMeshProUGUI currentWaveHeaderText;
    [SerializeField] private TextMeshProUGUI currentWaveText;
    [SerializeField] private TextMeshProUGUI waveProgressTimeText;
    [SerializeField] private TextMeshProUGUI currentHealthText;
    [SerializeField] private TextMeshProUGUI currentGoldText;

    [SerializeField] private Image waveProgressFillImage;

    [SerializeField] private TextMeshProUGUI gameEndText;
    [SerializeField] private Image gameEndBackgroundImage;

    public void InitializeUI()
    {
        GameManager.OnGameStart.RemoveListener(ResetText);
        GameManager.OnGameEnd.RemoveListener(OnGameEnd);
        ResetText();
        GameManager.OnGameStart.AddListener(ResetText);
        GameManager.OnGameEnd.AddListener(OnGameEnd);
    }

    private void Update()
    {
        if (GameManager.IsInActiveWave)
        {
            waveProgressFillImage.color = Color.yellow;
            float fixedProgress = GameManager.CurrentWaveProgress == 0f ? GameManager.CurrentWaveLength : GameManager.CurrentWaveProgress;
            currentWaveText.SetText("Wave: " + (GameManager.CurrentWaveCount + 1) + " / " + GameManager.TotalWaveCount);
            waveProgressTimeText.SetText(fixedProgress.ToString("F2") + " / " + GameManager.CurrentWaveLength.ToString("F2"));
            waveProgressFillImage.fillAmount = Mathf.InverseLerp(0, GameManager.CurrentWaveLength, fixedProgress);
        }
        else
        {
            waveProgressFillImage.color = Color.grey;
            waveProgressTimeText.SetText(GameManager.IntermissionProgress.ToString("F2") + " / " + GameManager.IntermissionLength.ToString("F2"));
            waveProgressFillImage.fillAmount = Mathf.InverseLerp(0, GameManager.IntermissionLength, GameManager.IntermissionProgress);
            currentWaveText.SetText("Intermission");
        }     
        currentHealthText.SetText(GameManager.Health.ToString());
        currentGoldText.SetText(GameManager.Currency.ToString());
    }

    private void ResetText()
    {
        gameEndBackgroundImage.enabled = false;
        gameEndText.enabled = false;
        currentWaveText.SetText(string.Empty);
        waveProgressTimeText.SetText(string.Empty);
        currentHealthText.SetText(string.Empty);
        currentGoldText.SetText(string.Empty);
    }

    private void OnGameEnd(bool didWin)
    {
        gameEndBackgroundImage.enabled = true;
        gameEndText.SetText("YOU " + (didWin ? "WON" : "LOST"));
        Color color = didWin ? Color.green : Color.red;
        gameEndText.color = color;
        gameEndBackgroundImage.color = new Color(color.r, color.g, color.b, 0.05f);
        gameEndText.enabled = true;
    }
}
