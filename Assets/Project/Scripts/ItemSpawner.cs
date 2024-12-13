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
            Spawn(Item);
    }

    public ItemBehaviour Spawn(ScriptableItem item = null)
    {
        if (item == null)
            item = Item;

        ItemBehaviour newItem = item.SpawnPrefab(null);
        if (item.ShouldBeAnchoredToGround && Physics.Raycast(transform.position, new Vector3(transform.position.x, -5000, transform.position.z), out RaycastHit hit, Mathf.Infinity, dropMask))
                newItem.transform.position = hit.point;
        else
            newItem.transform.position = transform.position;


        if (newItem.TryGetComponent(out IHittable hittable))
            GameManager.Instance.AllSpawnedHittables.Add(hittable);

        return newItem;
    }
}
