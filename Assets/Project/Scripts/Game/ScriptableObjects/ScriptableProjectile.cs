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
    [field: SerializeField] public float ImpactExplosionRadius { get; private set; }
    [field: SerializeField] public Color ProjectileColor { get; private set; }

    [field: Header("Projectile Visual Values"), Space(15)]
    [field: SerializeField] public ReactionInfo OnImpactReaction { get; private set; }


    public ProjectileBehaviour SpawnProjectile(Vector3 spawnPosition, Vector3 targetPosition, float force, int newDamage)
    {
        ProjectileBehaviour newProjectile = SpawnPrefab() as ProjectileBehaviour;
        newProjectile.transform.position = spawnPosition;
        newProjectile.ApplyProjectile(targetPosition, force, newDamage);

        return (newProjectile);
    }

    public override string GetCategoryName() => "Projectile";
}
