using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableWave", menuName = "TD-GJ/ScriptableWave", order = 1)]
public class ScriptableWave : ScriptableObject
{
    [field: SerializeField] public List<SpawnInfo> WaveSpawnInfos = new List<SpawnInfo>();
}
