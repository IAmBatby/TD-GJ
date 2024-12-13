using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableEnemy", menuName = "TD-GJ/ScriptableEnemy", order = 1)]
public class ScriptableEnemy : ScriptableObject
{
    [field: SerializeField] public EnemyAI EnemyPrefab { get; private set; }
    [field: SerializeField] public AudioPreset DamageAudioPreset { get; private set; }
    [field: SerializeField] public int Damage { get; private set; }
    [field: SerializeField] public int Health { get; private set; }
    [field: SerializeField] public float Speed { get; private set; }


    public EnemyAI SpawnEnemy(EnemySpawnTarget newSpawn, EnemyPathTarget newPath)
    {
        EnemyAI spawnedEnemy = GameObject.Instantiate(EnemyPrefab);
        spawnedEnemy.transform.position = newSpawn.transform.position;
        spawnedEnemy.InitializeEnemy(this, newPath);

        return (spawnedEnemy);
    }
}
