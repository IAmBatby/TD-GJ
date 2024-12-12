using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableUpgrade", menuName = "TD-GJ/ScriptableUpgrade", order = 1)]
public class ScriptableUpgrade : ScriptableItem
{
    [field: SerializeField] public ScriptableAttribute Attribute { get; private set; }
    [field: SerializeField] public float ModifierValue { get; private set; }
}
