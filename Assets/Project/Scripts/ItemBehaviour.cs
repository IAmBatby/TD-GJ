using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    [field: SerializeField] public ScriptableItem ItemData { get; private set; }
    [SerializeField] protected AudioSource primaryAudioSource;
    [field: SerializeField] public bool IsBeingHeld;

    public ExtendedEvent<bool> OnMouseoverToggle = new ExtendedEvent<bool>();


    private void OnMouseEnter()
    {
        if (ItemData.Cursor != null)
            GameManager.Player.RequestNewCursor(ItemData.Cursor);
        OnMouseoverToggle.Invoke(true);
    }
    private void OnMouseExit() => OnMouseoverToggle.Invoke(false);

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
