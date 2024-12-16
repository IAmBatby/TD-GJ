using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableSkin", menuName = "TD-GJ/ScriptableSkin", order = 1)]
public class ScriptableSkin : ScriptableObject
{
    [field: SerializeField] public GameObject SkinPrefab { get; private set; }
}
