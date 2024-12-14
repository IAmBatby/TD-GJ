using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviour : ContentBehaviour
{
    [field: SerializeField] public bool IsBeingHeld { get; private set; }

    public ScriptableItem ItemData { get; private set; }


    protected override void OnSpawn()
    {
        if (ContentData is ScriptableItem item)
            ItemData = item;
    }

    public void Pickup()
    {
        IsBeingHeld = true;
        AudioPlayer.PlayAudio(ItemData.OnPickupAudio);
        OnPickup();
    }

    public void Drop()
    {
        IsBeingHeld = false;
        AudioPlayer.PlayAudio(ItemData.OnDropAudio);
        OnDrop();
    }

    protected virtual void OnPickup() { }

    protected virtual void OnDrop() { }
}
