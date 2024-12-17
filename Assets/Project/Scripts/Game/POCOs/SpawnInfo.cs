using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnInfo
{
    [field: SerializeField] public float TimeToSpawn { get; private set; }
    [field: SerializeField] public ScriptableEnemy EnemyToSpawn { get; private set; }
    [field: SerializeField] public int AmountToSpawn { get; private set; }
}
