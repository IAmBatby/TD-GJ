using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    [SerializeField] public int maxHealth;
    private int currentHealth;

    public int Health { get => currentHealth; set => ModifyHealth(value); }

    private AudioPreset onHealthLostPreset;

    private AudioPlayer audioPlayer;

    public ExtendedEvent<(int, int)> OnHealthModified = new ExtendedEvent<(int, int)>();
    public ExtendedEvent OnDeath = new ExtendedEvent<(int, int)>();

    public HurtableBehaviour LinkedBehaviour { get; private set; }

    private ScriptableHurtable HurtableData => LinkedBehaviour?.HurtableData;

    private ContentDisplayInfo healthDisplayInfo;

    private void OnEnable()
    {
        GameManager.Instance.AllHealthControllers.Add(this);
        healthDisplayInfo = new ContentDisplayInfo(currentHealth.ToString(), null, Color.red);
        OnHealthModified.AddListener(RefreshUI);
    }

    private void OnDisable() => GameManager.Instance?.AllHealthControllers.Remove(this);

    private void Awake()
    {
        audioPlayer = AudioPlayer.Create(this);
    }

    public void LinkBehaviour(HurtableBehaviour behaviour, AudioPreset healthLostPreset)
    {
        LinkedBehaviour = behaviour;
        SetMaxHealth(HurtableData.Health);
        ResetHealth();
        LinkedBehaviour.AddContentDisplayInfo(healthDisplayInfo);
        RefreshUI();
    }

    public void ModifyHealth(int value)
    {
        int previousHealth = currentHealth;

        currentHealth = Mathf.Clamp(currentHealth + value, 0, maxHealth);

        if (currentHealth == 0)
            OnDeath.Invoke();

        if (currentHealth != previousHealth)
            audioPlayer.PlayAudio(currentHealth > previousHealth ? HurtableData.OnHealthGainedAudio : HurtableData.OnHealthLostAudio);

        OnHealthModified.Invoke((previousHealth, currentHealth));
    }

    public void SetMaxHealth(int newMaxHealth) => maxHealth = newMaxHealth;
    public void SetCurrentHealth(int newCurrentHealth) => ModifyHealth(-currentHealth + newCurrentHealth);
    public void ResetHealth() => ModifyHealth(-currentHealth + maxHealth);

    private void RefreshUI()
    {
        healthDisplayInfo.DisplayText = "HP: " + currentHealth.ToString();
        healthDisplayInfo.FillAmount = Mathf.InverseLerp(0, maxHealth, currentHealth);
    }
}
