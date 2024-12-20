using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviour : ContentBehaviour
{
    [field: SerializeField] public bool IsBeingHeld { get; private set; }

    public ScriptableItem ItemData { get; private set; }

    public ExtendedEvent OnItemPickup { get; private set; } = new ExtendedEvent();
    public ExtendedEvent OnItemDropped { get; private set; } = new ExtendedEvent();

    protected override void OnSpawn()
    {
        if (ContentData is ScriptableItem item)
            ItemData = item;
    }

    public void Pickup()
    {
        IsBeingHeld = true;
        ReactionPlayer.Play(ItemData.OnPickupReaction);
        OnItemPickup.Invoke();
        OnPickup();
    }

    public void Drop()
    {
        IsBeingHeld = false;
        ReactionPlayer.Play(ItemData.OnDropReaction);
        OnItemPickup.Invoke();
        OnDrop();
    }

    protected virtual void OnPickup() { }

    protected virtual void OnDrop() { }

    public override void RegisterBehaviour()
    {
        CheatManager.RegisterCheat(ScriptableContent.SpawnPrefab, ContentData, "Spawning");
        ContentManager.RegisterBehaviour(this);
        base.RegisterBehaviour();
    }
    public override void UnregisterBehaviour(bool destroyOnUnregistration)
    {
        ContentManager.UnregisterBehaviour(this, destroyOnUnregistration);
        base.UnregisterBehaviour(destroyOnUnregistration);
    }
}
