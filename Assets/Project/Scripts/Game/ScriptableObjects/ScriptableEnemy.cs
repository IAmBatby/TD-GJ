using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableEnemy", menuName = "TD-GJ/ScriptableContents/ScriptableHurtables/ScriptableEnemy", order = 1)]
public class ScriptableEnemy : ScriptableHurtable
{
    [field: Header("Enemy Values"), Space(15)]
    [field: SerializeField] public int Damage { get; private set; }
    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public Vector2 GoldDropRateMinMax { get; private set; }
    [field: SerializeField] public ScriptableItem ItemDrop { get; private set; }
    [field: SerializeField, Range(0,100)] public float ItemDropRate { get; private set; }
}
