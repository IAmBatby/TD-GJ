using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemySpawnInfo
{
    [field: SerializeField] public float TimeToStart { get; private set; }
    [field: SerializeField] public List<ScriptableEnemy> EnemiesToSpawn { get; private set; }
}
