using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableItem : ScriptableObject
{
    [field: SerializeField] public ItemBehaviour Prefab { get; private set; }

    public virtual ItemBehaviour SpawnPrefab(Transform parent = null)
    {
        ItemBehaviour spawnedBehaviour = GameObject.Instantiate(Prefab, parent);
        spawnedBehaviour.Initialize(this);
        return (SpawnPrefabSetup(spawnedBehaviour));
    }

    protected virtual ItemBehaviour SpawnPrefabSetup(ItemBehaviour newBehaviour)
    {
        return (newBehaviour);
    }
}
