using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighlightInfoElement : MonoBehaviour
{
    [SerializeField] private Transform parentTransform;

    [SerializeField] private HighlightCanvasInfo primaryInfo;
    [SerializeField] private HighlightCanvasInfo secondaryInfo;

    public void EnableHighlightInfo(ContentDisplayInfo newInfo)
    {
        parentTransform.gameObject.SetActive(true);
        enabled = true;
        primaryInfo.Apply(newInfo.PrimaryText, newInfo.DisplayColor, newInfo.GetFillRate(), newInfo.DisplayIcon, newInfo.DisplayMode == PresentationType.Progress || newInfo.DisplayMode == PresentationType.Percentage);
        secondaryInfo.Apply(newInfo.SecondaryText, newInfo.DisplayColor, newInfo.GetFillRate(), newInfo.DisplayIcon, newInfo.DisplayMode == PresentationType.Progress || newInfo.DisplayMode == PresentationType.Percentage);
    }

    public void DisableHighlightInfo()
    {
        parentTransform.gameObject.SetActive(false);
        primaryInfo.Disable();
        secondaryInfo.Disable();
        enabled = false;
    }
}

[System.Serializable]
public struct HighlightCanvasInfo
{
    [SerializeField] private Transform Parent;
    [SerializeField] private TextMeshProUGUI DisplayText;
    [SerializeField] private Image BackgroundImage;
    [SerializeField] private Image FillImage;
    [SerializeField] private Image IconImage;
    [SerializeField] private Transform ProgressMeterParent;

    public void Apply(string newText, Color newFillColor, float newFillRate, Sprite newIcon, bool showProgressMeter)
    {
        if (!string.IsNullOrEmpty(newText))
            Enable(newText, newFillColor, newFillRate, newIcon, showProgressMeter);
        else
            Disable();
    }

    public void Enable(string newTextValue, Color newFillColor, float newFillRate, Sprite newIcon, bool showProgressMeter)
    {
        Parent.gameObject.SetActive(true);
        DisplayText.SetText(newTextValue);
        FillImage.color = newFillColor;
        FillImage.fillAmount = newFillRate;
        IconImage.sprite = newIcon;
        ProgressMeterParent.gameObject.SetActive(showProgressMeter);
    }

    public void Disable()
    {
        Parent.gameObject.SetActive(false);
    }
}
