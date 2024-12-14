using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ContentDisplayInfo
{
    [field: SerializeField] public string DisplayText { get; set; }
    [field: SerializeField] public Sprite DisplayIcon { get; set; }
    [field: SerializeField] public Color DisplayColor { get; set; }
    [field: SerializeField] public float FillAmount { get; set; }

    public ContentDisplayInfo(string newDisplayText, Sprite newDisplayIcon, Color newDisplayColor)
    {
        DisplayText = newDisplayText;
        DisplayIcon = newDisplayIcon;
        DisplayColor = newDisplayColor;
        FillAmount = 100f;
    }
}
