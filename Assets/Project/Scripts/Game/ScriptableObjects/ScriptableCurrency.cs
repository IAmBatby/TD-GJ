using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableCurrency", menuName = "TD-GJ/ScriptableContents/ScriptableItems/ScriptableCurrency", order = 1)]
public class ScriptableCurrency : ScriptableItem
{
    [field: SerializeField] public Vector2 CurrencyAmount { get; private set; }
    [field: SerializeField, Range(0f,100f)] public float CurrencyWaveScale { get; private set; }    
}
