using IterationToolkit;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class HurtableBehaviour : ContentBehaviour
{
    public ScriptableHurtable HurtableData { get; private set; }

    public int MaxHealth { get; private set; }
    private int currentHealth;
    public int Health { get => currentHealth; set => ModifyHealth(value); }

    public ExtendedEvent<(int, int)> OnHealthModified = new ExtendedEvent<(int, int)>();
    public ExtendedEvent OnHurtableDeath = new ExtendedEvent<(int, int)>();

    protected ContentDisplayInfo HealthDisplayInfo { get; private set; }    

    protected override void OnSpawn()
    {
        base.OnSpawn();
        if (ContentData is ScriptableHurtable hurtableData)
            HurtableData = hurtableData;

        RefreshMaxHealth();
        //SetMaxHealth(HurtableData.Health);
        ResetHealth();
        HealthDisplayInfo = new ContentDisplayInfo(currentHealth.ToString(), displayMode: PresentationType.Progress, displayIcon: null, displayColor: Color.red);
        HealthDisplayInfo.DisplayMode = DisplayType.Mini;
        GeneralDisplayListing.AddContentDisplayInfo(HealthDisplayInfo);
        OnHealthModified.AddListener(RefreshDisplayInfo);
        GameManager.OnNewWave.AddListener(RefreshMaxHealth);
    }

    public void Die()
    {
        OnHurtableDeath.Invoke();
        OnDeath();
    }

    protected void RefreshMaxHealth()
    {
        SetMaxHealth(GameManager.WaveManifest.GetAdditiveHealth(HurtableData.Health, GameManager.CurrentWaveCount));
    }

    protected virtual void OnDeath() { }

    public void SetMaxHealth(int newMaxHealth) => MaxHealth = newMaxHealth;
    public void SetCurrentHealth(int newCurrentHealth) => ModifyHealth(-currentHealth + newCurrentHealth);
    public void ResetHealth() => ModifyHealth(-currentHealth + MaxHealth);

    public void ReceiveHit(int knockbackForce, int value, Vector3 velocity)
    {
        OnReceivedHit();

        Vector3 newVel = new Vector3(velocity.x, Rigidbody.velocity.y, velocity.z);

        Vector3 force = (newVel * knockbackForce * -value) / 10;

        Rigidbody.AddForce(OnBeforeHitForceApplied(force), ForceMode.VelocityChange);

        ModifyHealth(value);
    }

    protected virtual Vector3 OnBeforeHitForceApplied(Vector3 forceToBeHitWith) => forceToBeHitWith;

    protected virtual void OnReceivedHit()
    {

    }
    public void ModifyHealth(int value)
    {
        int previousHealth = currentHealth;

        currentHealth = Mathf.Clamp(currentHealth + value, 0, MaxHealth);

        if (currentHealth != previousHealth)
            ReactionPlayer.Play(currentHealth > previousHealth ? HurtableData.OnHealthGainedReaction : HurtableData.OnHealthLostReaction);

        OnHealthModified.Invoke((previousHealth, currentHealth));

        if (currentHealth == 0)
            Die();
    }

    public virtual bool CanDie() => true;

    private void RefreshDisplayInfo()
    {
        HealthDisplayInfo.SetDisplayValues(currentHealth.ToString());
        HealthDisplayInfo.SetProgressValues(currentHealth, MaxHealth);
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
