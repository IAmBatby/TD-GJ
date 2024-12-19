using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableHurtable", menuName = "TD-GJ/ScriptableContents/ScriptableHurtables/ScriptableHurtable", order = 1)]
public class ScriptableHurtable : ScriptableContent
{
    [field: Header("Hurtable Values"), Space(15)]
    [field: SerializeField] public int Health { get; private set; }
    [field: SerializeField, Range(0f,100f)] public float HealthWaveScale { get; private set; }

    [field: Header("Hurtable Visual Values"), Space(15)]
    [field: SerializeField] public ReactionInfo OnHealthLostReaction { get; private set; }
    [field: SerializeField] public ReactionInfo OnHealthGainedReaction { get; private set; }

    public override string GetCategoryName() => "Hurtable";
}
