using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableProjectile", menuName = "TD-GJ/ScriptableProjectile", order = 1)]
public class ScriptableProjectile : ScriptableObject
{
    [field: SerializeField] public ProjectileBehaviour Prefab { get; private set; }


    public ProjectileBehaviour SpawnProjectile(Vector3 spawnPosition, Vector3 targetPosition, float force, int newDamage)
    {
        ProjectileBehaviour instancedProjectile = GameManager.Instantiate(Prefab);
        instancedProjectile.transform.position = spawnPosition;
        instancedProjectile.Initialize(this, targetPosition, force, newDamage);

        return (instancedProjectile);
    }
}
