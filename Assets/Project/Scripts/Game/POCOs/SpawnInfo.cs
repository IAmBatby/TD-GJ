using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnInfo
{
    [field: SerializeField] public float TimeToSpawn { get; private set; }
    [field: SerializeField] public ScriptableEnemy EnemyToSpawn { get; private set; }
    [field: SerializeField] public int AmountToSpawn { get; private set; }
    [field: SerializeField] public float IntervalBetweenSpawns { get; private set; }


    public bool IsValidSpawnRequest()
    {
        if (EnemyToSpawn == null || AmountToSpawn == 0 || TimeToSpawn < 0)
            return (false);
        return (true);
    }
}
