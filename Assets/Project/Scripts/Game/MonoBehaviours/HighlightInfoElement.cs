using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighlightInfoElement : MonoBehaviour
{
    [SerializeField] private Transform parentTransform;
    [SerializeField] private Image fillAmountImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconGame;
    [SerializeField] private TextMeshProUGUI contentBehaviourName;

    public void EnableHighlightInfo(ContentDisplayInfo newInfo)
    {
        parentTransform.gameObject.SetActive(true);
        enabled = true;
        contentBehaviourName.SetText(newInfo.DisplayText);
        iconGame.sprite = newInfo.DisplayIcon;
        fillAmountImage.color = newInfo.DisplayColor;
        fillAmountImage.fillAmount = newInfo.FillAmount;
    }

    public void DisableHighlightInfo()
    {
        parentTransform.gameObject.SetActive(false);
        enabled = false;
    }
}
