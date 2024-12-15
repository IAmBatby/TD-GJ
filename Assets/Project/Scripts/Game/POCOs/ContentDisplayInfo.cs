using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public enum PresentationType { Standard, Progress, Percentage }
public class ContentDisplayInfo
{
    public Sprite DisplayIcon { get; set; }
    public Color DisplayColor { get; set; }
    public PresentationType DisplayMode { get; set; } = PresentationType.Standard;

    public string PrimaryText { get; private set; }
    public string SecondaryText { get; private set; }

    private float progressMinValue;
    private float progressMaxValue;
    private float progressCurrentValue;

    public ContentDisplayInfo(string primaryText, string secondaryText = null, Color displayColor = default, PresentationType displayMode = PresentationType.Standard, Sprite displayIcon = null)
    {
        SetDisplayValues(primaryText, secondaryText);
        DisplayColor = displayColor;
        DisplayMode = displayMode;
        DisplayIcon = displayIcon;

        progressMinValue = 0f;
        progressMaxValue = 1f;
        progressCurrentValue = 1f;
    }

    public void SetDisplayValues(string newPrimaryText = null, string newSecondaryText = null)
    {
        PrimaryText = newPrimaryText != null ? newPrimaryText : string.Empty;
        SecondaryText = newSecondaryText != null ? newSecondaryText : string.Empty;
    }

    public void SetProgressValues(float newCurrentValue, float newMaxValue, float newMinValue = 0f)
    {
        progressMinValue = newMinValue;
        progressMaxValue = newMaxValue;
        progressCurrentValue = newCurrentValue;
    }

    public string GetDisplayText()
    {
        return (DisplayMode switch
        {
            PresentationType.Standard => string.IsNullOrEmpty(SecondaryText) ? PrimaryText : PrimaryText + " (" + SecondaryText + ")",
            PresentationType.Progress => string.IsNullOrEmpty(SecondaryText) ? PrimaryText : PrimaryText + " / " + SecondaryText,
            PresentationType.Percentage => PrimaryText + "%",
            _ => string.Empty
        });
    }

    public float GetFillRate() => Mathf.InverseLerp(progressMinValue, progressMaxValue, progressCurrentValue);
}