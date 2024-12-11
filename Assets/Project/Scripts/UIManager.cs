using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : GlobalManager
{
    public static new UIManager Instance => SingletonManager.GetSingleton<UIManager>(typeof(UIManager));

    [SerializeField] private TextMeshProUGUI currentWaveText;
    [SerializeField] private TextMeshProUGUI waveProgressTimeText;
    [SerializeField] private TextMeshProUGUI currentHealthText;
    [SerializeField] private TextMeshProUGUI currentGoldText;


    private void Update()
    {
        currentWaveText.SetText((GameManager.Instance.CurrentWaveCount + 1) + " / " + GameManager.Instance.TotalWaveCount);
        waveProgressTimeText.SetText(GameManager.Instance.CurrentWaveTime + " / " + GameManager.Instance.TotalWaveTime);
        currentHealthText.SetText(GameManager.Instance.Health.ToString());
    }
}
