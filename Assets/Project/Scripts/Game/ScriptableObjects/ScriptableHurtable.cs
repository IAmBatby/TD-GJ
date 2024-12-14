using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableHurtable", menuName = "TD-GJ/ScriptableContents/ScriptableHurtables/ScriptableHurtable", order = 1)]
public class ScriptableHurtable : ScriptableContent
{
    [field: Header("Hurtable Values"), Space(15)]
    [field: SerializeField] public int Health { get; private set; }

    [field: Header("Hurtable Audio Values"), Space(15)]
    [field: SerializeField] public AudioPreset OnHealthLostAudio { get; private set; }
    [field: SerializeField] public AudioPreset OnHealthGainedAudio { get; private set; }
}
