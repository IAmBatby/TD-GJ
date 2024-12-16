using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighlightInfoElement : MonoBehaviour
{
    [SerializeField] private RectTransform parentTransform;

    [SerializeField] private HighlightCanvasInfo fullForm;
    [SerializeField] private HighlightCanvasInfo compressedForm;

    public void EnableHighlightInfo(ContentDisplayInfo newInfo)
    {
        parentTransform.gameObject.SetActive(true);
        enabled = true;
        if (newInfo.DisplayMode == DisplayType.Full)
        {
            parentTransform.sizeDelta = fullForm.RectSize;
            fullForm.Enable(newInfo.PrimaryText, newInfo.DisplayColor, newInfo.GetFillRate(), newInfo.DisplayIcon, newInfo.PresentMode == PresentationType.Progress || newInfo.PresentMode == PresentationType.Percentage, newInfo.DisplayScale);
            compressedForm.Disable();
        }
        else
        {
            parentTransform.sizeDelta = compressedForm.RectSize;
            compressedForm.Enable(newInfo.PrimaryText, newInfo.DisplayColor, newInfo.GetFillRate(), newInfo.DisplayIcon, newInfo.PresentMode == PresentationType.Progress || newInfo.PresentMode == PresentationType.Percentage, newInfo.DisplayScale);
            fullForm.Disable();
        }
    }

    public void DisableHighlightInfo()
    {
        parentTransform.gameObject.SetActive(false);
        fullForm.Disable();
        compressedForm.Disable();
        enabled = false;
    }
}

[System.Serializable]
public struct HighlightCanvasInfo
{
    [SerializeField] private RectTransform Parent;
    [SerializeField] private TextMeshProUGUI DisplayText;
    [SerializeField] private Image BackgroundImage;
    [SerializeField] private Image FillImage;
    [SerializeField] private Image IconImage;
    [SerializeField] private Transform ProgressMeterParent;
    [field: SerializeField] public Vector2 RectSize { get; private set; }

    public void Apply(string newText, Color newFillColor, float newFillRate, Sprite newIcon, bool showProgressMeter, Vector2 displayScale)
    {
        if (!string.IsNullOrEmpty(newText))
            Enable(newText, newFillColor, newFillRate, newIcon, showProgressMeter, displayScale);
        else
            Disable();
    }

    public void Enable(string newTextValue, Color newFillColor, float newFillRate, Sprite newIcon, bool showProgressMeter, Vector2 displayScale)
    {
        Parent.gameObject.SetActive(true);
        DisplayText.SetText(newTextValue);
        FillImage.color = newFillColor;
        FillImage.fillAmount = newFillRate;
        IconImage.sprite = newIcon;
        IconImage.enabled = IconImage.sprite != null;
        ProgressMeterParent.gameObject.SetActive(showProgressMeter);
    }

    public void Disable()
    {
        Parent.gameObject.SetActive(false);
    }
}
