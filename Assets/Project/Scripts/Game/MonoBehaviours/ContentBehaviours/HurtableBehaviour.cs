using IterationToolkit;
using UnityEngine;

public class HurtableBehaviour : ContentBehaviour
{
    public ScriptableHurtable HurtableData { get; private set; }

    public int MaxHealth { get; private set; }
    private int currentHealth;
    public int Health { get => currentHealth; set => ModifyHealth(value); }

    public ExtendedEvent<(int, int)> OnHealthModified = new ExtendedEvent<(int, int)>();
    public ExtendedEvent OnHurtableDeath = new ExtendedEvent<(int, int)>();

    private ContentDisplayInfo healthDisplayInfo;

    protected override void OnSpawn()
    {
        base.OnSpawn();
        if (ContentData is ScriptableHurtable hurtableData)
            HurtableData = hurtableData;

        RefreshMaxHealth();
        //SetMaxHealth(HurtableData.Health);
        ResetHealth();
        healthDisplayInfo = new ContentDisplayInfo(currentHealth.ToString(), displayMode: PresentationType.Progress, displayIcon: null, displayColor: Color.red);
        healthDisplayInfo.DisplayMode = DisplayType.Mini;
        GeneralDisplayListing.AddContentDisplayInfo(healthDisplayInfo);
        OnHealthModified.AddListener(RefreshDisplayInfo);
        GameManager.OnNewWave.AddListener(RefreshMaxHealth);
    }

    public void Die()
    {
        OnHurtableDeath.Invoke();
        OnDeath();
    }

    private void RefreshMaxHealth()
    {
        SetMaxHealth(Mathf.RoundToInt(Utilities.GetScaledValue(HurtableData.Health, HurtableData.HealthWaveScale, GameManager.Instance.CurrentWaveCount)));
    }

    protected virtual void OnDeath() { }

    public void SetMaxHealth(int newMaxHealth) => MaxHealth = newMaxHealth;
    public void SetCurrentHealth(int newCurrentHealth) => ModifyHealth(-currentHealth + newCurrentHealth);
    public void ResetHealth() => ModifyHealth(-currentHealth + MaxHealth);

    public void ModifyHealth(int value)
    {
        int previousHealth = currentHealth;

        currentHealth = Mathf.Clamp(currentHealth + value, 0, MaxHealth);

        if (currentHealth == 0)
            Die();

        if (currentHealth != previousHealth)
            AudioPlayer.PlayAudio(currentHealth > previousHealth ? HurtableData.OnHealthGainedAudio : HurtableData.OnHealthLostAudio);

        OnHealthModified.Invoke((previousHealth, currentHealth));
    }

    public virtual bool CanDie() => true;

    private void RefreshDisplayInfo()
    {
        healthDisplayInfo.SetDisplayValues(currentHealth.ToString());
        healthDisplayInfo.SetProgressValues(currentHealth, MaxHealth);
    }

    public override void RegisterBehaviour()
    {
        ContentManager.RegisterBehaviour(this);
        base.RegisterBehaviour();
    }
    public override void UnregisterBehaviour(bool destroyOnUnregistration)
    {
        ContentManager.UnregisterBehaviour(this, destroyOnUnregistration);
        base.UnregisterBehaviour(destroyOnUnregistration);
    }
}
