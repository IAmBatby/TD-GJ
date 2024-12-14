using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtableBehaviour : ContentBehaviour, IHittable
{
    [field: Header("Required References")]
    [field: SerializeField] public HealthController HealthController { get; private set; }

    public ScriptableHurtable HurtableData { get; private set; }

    public int Health { get => HealthController.Health; set => HealthController.ModifyHealth(value); }

    protected override void OnSpawn()
    {
        base.OnSpawn();
        if (ContentData is ScriptableHurtable hurtableData)
            HurtableData = hurtableData;
        
        HealthController.LinkBehaviour(this, HurtableData.OnHealthLostAudio);
        HealthController.SetMaxHealth(HurtableData.Health);
        HealthController.ResetHealth();
        HealthController.OnDeath.AddListener(Die);
    }

    public void Die()
    {
        OnDeath();
    }

    protected virtual void OnDeath() { }

    public void RecieveHit(int value) => HealthController.ModifyHealth(value);
    public Transform GetTransform() => transform;
}
