using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableItem", menuName = "TD-GJ/ScriptableContents/ScriptableItems/ScriptableItem", order = 1)]
public class ScriptableItem : ScriptableContent
{
    [field: Header("Item Values"), Space(15)]
    [field: SerializeField] public AudioPreset OnPickupAudio { get; private set; }
    [field: SerializeField] public AudioPreset OnDropAudio{ get; private set; }
    [field: SerializeField] public ReactionInfo OnPickupReaction { get; private set; }
    [field: SerializeField] public ReactionInfo OnDropReaction { get; private set; }
    [field: SerializeField] public bool CanBePickedUp { get; private set; } //Might replace with a bool function later
}
