using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableUpgrade", menuName = "TD-GJ/ScriptableContents/ScriptableItems/ScriptableUpgrade", order = 1)]
public class ScriptableUpgrade : ScriptableItem
{
    [field: Header("Upgrade Values"), Space(15)]
    [field: SerializeField] public ScriptableFloatAttribute Attribute { get; private set; }
    [field: SerializeField] public float ModifierValue { get; private set; }
}