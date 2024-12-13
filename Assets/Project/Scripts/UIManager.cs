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
        GameManager.Instance.OnGameStart.RemoveListener(ResetText);
        GameManager.Instance.OnGameEnd.RemoveListener(OnGameEnd);
        ResetText();
        GameManager.Instance.OnGameStart.AddListener(ResetText);
        GameManager.Instance.OnGameEnd.AddListener(OnGameEnd);
    }

    private void Update()
    {
        if (GameManager.Instance.IsInIntermission == false)
        {
            waveProgressFillImage.color = Color.yellow;
            currentWaveText.SetText("Wave: " + (GameManager.Instance.CurrentWaveCount + 1) + " / " + GameManager.Instance.TotalWaveCount);
            waveProgressTimeText.SetText(GameManager.Instance.CurrentWaveTime.ToString("F2") + " / " + GameManager.Instance.TotalWaveTime.ToString("F2"));
            if (GameManager.Instance.HasWaveTimeFinished == false)
                waveProgressFillImage.fillAmount = Mathf.InverseLerp(0, GameManager.Instance.TotalWaveTime, GameManager.Instance.CurrentWaveTime);
            else
                waveProgressFillImage.fillAmount = 1f;
        }
        else
        {
            waveProgressFillImage.color = Color.grey;
            waveProgressTimeText.SetText(GameManager.Instance.IntermissionProgress.ToString("F2") + " / " + GameManager.Instance.IntermissionLength.ToString("F2"));
            waveProgressFillImage.fillAmount = Mathf.InverseLerp(0, GameManager.Instance.IntermissionLength, GameManager.Instance.IntermissionProgress);
            currentWaveText.SetText("Intermission");
        }

        
        currentHealthText.SetText(GameManager.Instance.Health.ToString());
        currentGoldText.SetText(GameManager.Instance.Currency.ToString());


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

        if (didWin == true)
        {
            gameEndText.SetText("YOU WIN");
            gameEndText.color = Color.green;
            gameEndBackgroundImage.color = new Color(Color.green.r, Color.green.g, Color.green.b, 0.05f);
        }
        else
        {
            gameEndText.SetText("YOU LOST");
            gameEndText.color = Color.red;
            gameEndBackgroundImage.color = new Color(Color.red.r, Color.red.g, Color.red.b, 0.05f);
        }

        gameEndText.enabled = true;
    }
}
