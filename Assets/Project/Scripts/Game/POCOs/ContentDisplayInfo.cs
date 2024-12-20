using UnityEngine;

public enum PresentationType { Standard, Progress, Percentage }
public enum DisplayType { Full, Mini }
public class ContentDisplayInfo
{
    public Sprite DisplayIcon { get; set; }
    public Color DisplayColor { get; set; }
    public PresentationType PresentMode { get; set; } = PresentationType.Standard;
    public DisplayType DisplayMode { get; set; } = DisplayType.Full;

    public string PrimaryText { get; private set; }

    private float progressMinValue;
    private float progressMaxValue;
    private float progressCurrentValue;

    public Vector2 DisplayScale { get; set; }

    public ContentDisplayInfo(string primaryText, string secondaryText = null, Color displayColor = default, PresentationType displayMode = PresentationType.Standard, Sprite displayIcon = null)
    {
        SetDisplayValues(primaryText);
        DisplayColor = displayColor;
        PresentMode = displayMode;
        DisplayIcon = displayIcon;

        progressMinValue = 0f;
        progressMaxValue = 1f;
        progressCurrentValue = 1f;

        DisplayScale = new Vector2(1f, 1f);
    }

    public void SetDisplayValues(string newPrimaryText = null)
    {
        PrimaryText = newPrimaryText != null ? newPrimaryText : string.Empty;
    }

    public void SetProgressValues(float newCurrentValue, float newMaxValue, float newMinValue = 0f)
    {
        progressMinValue = newMinValue;
        progressMaxValue = newMaxValue;
        progressCurrentValue = newCurrentValue;
    }

    public string GetDisplayText()
    {
        return (PresentMode switch
        {
            PresentationType.Standard => PrimaryText,
            PresentationType.Progress => progressCurrentValue.ToString("F2") + " / " + progressMaxValue.ToString("F2"),
            PresentationType.Percentage => PrimaryText + "%",
            _ => string.Empty
        });
    }

    public float GetFillRate() => Mathf.InverseLerp(progressMinValue, progressMaxValue, progressCurrentValue);
}