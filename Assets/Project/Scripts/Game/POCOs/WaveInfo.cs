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
                for (int i = 0; i < enemyInfo.AmountToSpawn; i++)
                    list.Add(enemyInfo.EnemyToSpawn);
            else
            {
                List<ScriptableEnemy> newList = new List<ScriptableEnemy>();
                for (int i = 0; i < enemyInfo.AmountToSpawn; i++)
                    newList.Add(enemyInfo.EnemyToSpawn);
                enemyDict.Add(enemyInfo.TimeToStart, newList);
            }

        }

        return (enemyDict);
    }
}
