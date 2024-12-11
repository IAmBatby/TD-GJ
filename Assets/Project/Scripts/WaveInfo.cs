using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WaveInfo
{
    [field: SerializeField] public float WaveLength { get; private set; }
    [field: SerializeField] public List<EnemySpawnInfo> EnemySpawnInfos { get; private set; } = new List<EnemySpawnInfo>();
}
