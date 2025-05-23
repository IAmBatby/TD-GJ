using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableLevel", menuName = "TD-GJ/ScriptableLevel", order = 1)]
public class ScriptableLevel : ScriptableObject
{
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public string SceneName { get; private set; }

    [field: SerializeField] public int StartingCurrency { get; private set; }
    [field: SerializeField] public int StartingHealth { get; private set; }

    [field: SerializeField] public WaveManifest WaveManifest { get; private set; }


}
