using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    [field: SerializeField] public ScriptableItem ItemData { get; private set; }
    [field: SerializeField] public bool IsBeingHeld;

    public void Initialize(ScriptableItem item)
    {
        ItemData = item;
        OnSpawn();
    }

    protected virtual void OnSpawn()
    {

    }
}
