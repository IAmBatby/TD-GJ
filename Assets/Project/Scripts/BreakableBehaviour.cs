using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BreakableBehaviour : ItemBehaviour
{
    [SerializeField] private HealthController healthController;
    [SerializeField] private AudioPreset onDamageTakenAudioPreset;
    [SerializeField] private int Health;

    [SerializeField] private UnityEvent onDamageTaken;
    [SerializeField] private UnityEvent onDeath;

    private void Awake()
    {
        healthController.LinkBehaviour(this, onDamageTakenAudioPreset);
        healthController.SetMaxHealth(Health);
        healthController.ResetHealth();
        healthController.OnHealthModified.AddListener(onDamageTaken.Invoke);
        healthController.OnDeath.AddListener(onDeath.Invoke);
    }
}
