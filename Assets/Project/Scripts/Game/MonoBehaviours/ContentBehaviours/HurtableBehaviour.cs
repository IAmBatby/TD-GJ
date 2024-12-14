using IterationToolkit;
using UnityEngine;

public class HurtableBehaviour : ContentBehaviour, IHittable
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

        //GameManager.Instance.AllHealthControllers.Add(this); Will Need To Fix!!
        SetMaxHealth(HurtableData.Health);
        ResetHealth();
        healthDisplayInfo = new ContentDisplayInfo(currentHealth.ToString(), null, Color.red);
        AddContentDisplayInfo(healthDisplayInfo);
        OnHealthModified.AddListener(RefreshDisplayInfo);
    }

    public void Die()
    {
        OnHurtableDeath.Invoke();
        OnDeath();
    }

    protected virtual void OnDeath() { }

    public void RecieveHit(int value) => ModifyHealth(value);
    public Transform GetTransform() => transform;

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

    private void RefreshDisplayInfo()
    {
        healthDisplayInfo.DisplayText = "HP: " + currentHealth.ToString();
        healthDisplayInfo.FillAmount = Mathf.InverseLerp(0, MaxHealth, currentHealth);
    }
}
