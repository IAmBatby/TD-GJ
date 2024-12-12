using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WaveInfo
{
    [field: SerializeField] public float WaveLength { get; private set; }
    [field: SerializeField] public List<EnemySpawnInfo> EnemySpawnInfos { get; private set; } = new List<EnemySpawnInfo>();

    public Dictionary<float, List<ScriptableEnemy>> GetEnemyDict()
    {
        Dictionary<float, List<ScriptableEnemy>> enemyDict = new Dictionary<float, List<ScriptableEnemy>>();

        foreach (EnemySpawnInfo enemyInfo in EnemySpawnInfos)
        {
            if (enemyDict.TryGetValue(enemyInfo.TimeToStart, out List<ScriptableEnemy> list))
                list.AddRange(enemyInfo.EnemiesToSpawn);
            else
                enemyDict.Add(enemyInfo.TimeToStart, new List<ScriptableEnemy>(enemyInfo.EnemiesToSpawn));
        }

        return (enemyDict);
    }
}
