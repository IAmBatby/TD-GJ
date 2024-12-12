using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : GlobalManager
{
    public static new UIManager Instance => SingletonManager.GetSingleton<UIManager>(typeof(UIManager));

    [SerializeField] private TextMeshProUGUI currentWaveText;
    [SerializeField] private TextMeshProUGUI waveProgressTimeText;
    [SerializeField] private TextMeshProUGUI currentHealthText;
    [SerializeField] private TextMeshProUGUI currentGoldText;

    [SerializeField] private Image waveProgressFillImage;

    [SerializeField] private TextMeshProUGUI gameEndText;
    [SerializeField] private Image gameEndBackgroundImage;

    protected override void Awake()
    {
        base.Awake();
        ResetText();
        GameManager.Instance.OnGameStart.AddListener(ResetText);
        GameManager.Instance.OnGameEnd.AddListener(OnGameEnd);
    }

    private void Update()
    {
        currentWaveText.SetText((GameManager.Instance.CurrentWaveCount + 1) + " / " + GameManager.Instance.TotalWaveCount);
        waveProgressTimeText.SetText(GameManager.Instance.CurrentWaveTime.ToString("F2") + " / " + GameManager.Instance.TotalWaveTime.ToString("F2"));
        currentHealthText.SetText(GameManager.Instance.Health.ToString());

        waveProgressFillImage.fillAmount = Mathf.InverseLerp(0, GameManager.Instance.TotalWaveTime, GameManager.Instance.CurrentWaveTime);
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
            gameEndBackgroundImage.color = new Color(Color.green.r, Color.green.g, Color.green.b, 0.3f);
        }
        else
        {
            gameEndText.SetText("YOU LOST");
            gameEndText.color = Color.red;
            gameEndBackgroundImage.color = new Color(Color.red.r, Color.red.g, Color.red.b, 0.3f);
        }

        gameEndText.enabled = true;
    }
}
