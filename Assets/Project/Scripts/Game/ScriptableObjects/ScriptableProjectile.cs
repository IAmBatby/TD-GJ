using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableProjectile", menuName = "TD-GJ/ScriptableContents/ScriptableProjectile", order = 1)]
public class ScriptableProjectile : ScriptableContent
{
    [field: Header("Projectile Values"), Space(15)]
    [field: SerializeField] public bool IsPiercing { get; private set; }
    [field: SerializeField] public float LifeTime { get; private set; }

    [field: Header("Projectile Audio Values"), Space(15)]
    [field: SerializeField] public AudioPreset OnCollisionAudio { get; private set; }


    public ProjectileBehaviour SpawnProjectile(Vector3 spawnPosition, Vector3 targetPosition, float force, int newDamage)
    {
        ProjectileBehaviour newProjectile = SpawnPrefab() as ProjectileBehaviour;
        newProjectile.transform.position = spawnPosition;
        newProjectile.ApplyProjectile(targetPosition, force, newDamage);

        return (newProjectile);
    }
}