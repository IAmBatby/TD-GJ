using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    [field: SerializeField] public ScriptableItem ItemData { get; private set; }
    [SerializeField] protected AudioSource primaryAudioSource;
    [field: SerializeField] public bool IsBeingHeld;

    public void Initialize(ScriptableItem item)
    {
        ItemData = item;
        if (ItemData.OnSpawnAudioPreset != null)
            AudioManager.PlayAudio(ItemData.OnSpawnAudioPreset, primaryAudioSource);
        OnSpawn();
    }

    public void Pickup()
    {
        if (ItemData.OnPickupAudioPreset != null)
            AudioManager.PlayAudio(ItemData.OnPickupAudioPreset, primaryAudioSource);
        OnPickup();
    }

    public void Drop()
    {
        if (ItemData.OnDropAudioPreset != null)
            AudioManager.PlayAudio(ItemData.OnDropAudioPreset, primaryAudioSource);
        OnDrop();
    }

    protected virtual void OnSpawn() { }

    protected virtual void OnPickup() { }

    protected virtual void OnDrop() { }
}
