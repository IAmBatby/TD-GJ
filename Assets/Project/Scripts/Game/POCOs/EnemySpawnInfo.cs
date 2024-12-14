using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemySpawnInfo
{
    [field: SerializeField] public float TimeToStart { get; private set; }
    [field: SerializeField] public ScriptableEnemy EnemyToSpawn { get; private set; }
    [field: SerializeField] public int AmountToSpawn { get; private set; }
}
