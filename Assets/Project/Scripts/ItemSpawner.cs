using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [field: SerializeField] public bool SpawnOnAwake { get; private set; }
    [field: SerializeField] public ScriptableItem Item { get; private set; }
    [SerializeField] private LayerMask dropMask;

    private void Awake()
    {
        if (SpawnOnAwake)
            Spawn();
    }

    public void Spawn()
    {
        ItemBehaviour newItem = Item.SpawnPrefab(transform);
        if (Physics.Raycast(transform.position, new Vector3(transform.position.x, -5000, transform.position.z), out RaycastHit hit, Mathf.Infinity, dropMask))
            newItem.transform.position = hit.point;
        if (newItem.TryGetComponent(out IHittable hittable))
            GameManager.Instance.AllSpawnedHittables.Add(hittable);
    }
}
