using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [field: SerializeField] public bool SpawnOnAwake { get; private set; }
    [field: SerializeField] public ScriptableItem Item { get; private set; }

    private void Awake()
    {
        if (SpawnOnAwake)
            Item.SpawnPrefab(transform);
    }
}
