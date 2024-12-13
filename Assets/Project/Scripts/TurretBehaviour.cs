using IterationToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TargetType { Closest, Furthest, First, Last }
public class TurretBehaviour : ItemBehaviour
{
    private ScriptableTurret turretData;
    [field: SerializeField] public ScriptableProjectile Projectile { get; private set; }

    public List<ScriptableAttribute> AllAttributes = new List<ScriptableAttribute>();

    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue FireRateAttribute { get; private set; }
    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue ShotSpeedAttribute { get; private set; }
    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue RangeAttribute { get; private set; }
    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue DamageAttribute { get; private set; }

    public TargetType SelectedTargetType { get; private set; }
    //[SerializeField] private Transform shootPosition;
    [SerializeField] private LayerMask shootMask;
    [SerializeField] private List<ShootPosition> shootPositions;

    private HealthController Target;
    [SerializeField] private List<HealthController> AllEnemiesInRange = new List<HealthController>();

    [SerializeField] private List<HealthController> BlacklistedTargets = new List<HealthController>();

    private Timer shootCooldownTimer;
    private bool canShoot;

    protected override void OnSpawn()
    {
        canShoot = true;
        turretData = ItemData as ScriptableTurret;
        Projectile = turretData.Projectile;
        FireRateAttribute.ApplyWithReference(turretData.FireRateAttribute);
        ShotSpeedAttribute.ApplyWithReference(turretData.ShotSpeedAttribute);
        RangeAttribute.ApplyWithReference(turretData.RangeAttribute);
        DamageAttribute.ApplyWithReference(turretData.DamageAttribute);
        AllAttributes = new List<ScriptableAttribute>() { FireRateAttribute.Attribute, ShotSpeedAttribute.Attribute, RangeAttribute.Attribute, DamageAttribute.Attribute };
        SelectedTargetType = turretData.TargetType;

        BlacklistTarget(GameManager.Player.HealthController);

    }

    private void BlacklistTarget(HealthController hittable)
    {
        BlacklistedTargets.Add(hittable);
    }

    private void Update()
    {
        if (IsBeingHeld) return;

        HealthController previousTarget = Target;

        UpdateTargets();

        if (Target != null && previousTarget == null)
        {
            shootCooldownTimer = null;
            canShoot = true;
        }

        if (Target != null && Target != previousTarget)
            AudioManager.PlayAudio(turretData.OnNewTargetAudioPreset, primaryAudioSource);

        foreach (ShootPosition shootPosition in shootPositions)
            shootPosition.UpdateShootRenderer(Target);

        if (Target != null)
            transform.LookAt(Target.transform.position);
        else
            transform.rotation = Quaternion.Euler(0, 0, 0);

        Shoot();
    }

    private void Shoot()
    {
        if (Target == null || canShoot == false) return;

        bool didShoot = false;
        foreach (ShootPosition shootPosition in shootPositions)
            if (Physics.Raycast(shootPosition.transform.position, Target.LinkedBehaviour.transform.position, Mathf.Infinity, shootMask))
            {
                Projectile.SpawnProjectile(shootPosition.transform.position, Target.LinkedBehaviour.transform.position, ShotSpeedAttribute.Value, Mathf.RoundToInt(DamageAttribute.Value));
                didShoot = true;
            }

        if (didShoot == true)
        {
            AudioManager.PlayAudio(turretData.OnShootAudioPreset, primaryAudioSource);
            if (FireRateAttribute.Value > 0f)
            {
                canShoot = false;
                shootCooldownTimer = new Timer();
                shootCooldownTimer.onTimerEnd.AddListener(DisableCooldown);
                shootCooldownTimer.StartTimer(this, FireRateAttribute.Value);
            }
        }
    }

    private void DisableCooldown()
    {
        AudioManager.PlayAudio(turretData.OnCooldownEndedAudioPreset, primaryAudioSource);
        canShoot = true;
    }



    private void UpdateTargets()
    {
        AllEnemiesInRange.Clear();

        HealthController furthestEnemy = null;
        HealthController closestEnemy = null;
        HealthController firstEnemy = null;
        HealthController lastEnemy = null;

        float furthestDistance = Mathf.NegativeInfinity;
        float closestDistance = Mathf.Infinity;
        float closestDestinationRemaining = Mathf.Infinity;
        float furthestDestinationRemaining = Mathf.Infinity;

        foreach (HealthController hittable in GameManager.Instance.AllHealthControllers)
        {
            if (BlacklistedTargets.Contains(hittable)) continue;
            float distance = Vector3.Distance(transform.position, hittable.transform.position);
            if (distance <= RangeAttribute.Value)
            {
                AllEnemiesInRange.Add(hittable);
                if (closestEnemy == null || distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = hittable;
                }

                if (furthestEnemy == null || distance < furthestDistance)
                {
                    furthestDistance = distance;
                    closestEnemy = hittable;
                }

                if (hittable.LinkedBehaviour is EnemyAI enemy)
                {
                    if (firstEnemy == null || enemy.Agent.remainingDistance < closestDestinationRemaining)
                    {
                        firstEnemy = hittable;
                        closestDestinationRemaining = enemy.Agent.remainingDistance;
                    }

                    if (lastEnemy == null || enemy.Agent.remainingDistance > furthestDestinationRemaining)
                    {
                        lastEnemy = hittable;
                        furthestDestinationRemaining = enemy.Agent.remainingDistance;
                    }
                }
            }
        }

        Target = SelectedTargetType switch
        {
            TargetType.First => firstEnemy,
            TargetType.Last => lastEnemy,
            TargetType.Closest => closestEnemy,
            TargetType.Furthest => furthestEnemy,
            _ => null
        };

        if (Target == null && AllEnemiesInRange.Count > 0)
            Target = AllEnemiesInRange.First();
    }

    protected override void OnPickup()
    {
        Debug.Log("Turret Being Picked UP!");
    }


    private void OnDrawGizmosSelected()
    {
        IterationToolkit.Utilities.DrawCircle(transform.position, RangeAttribute.Value, Color.yellow);

        foreach (HealthController health in GameManager.Instance.AllHealthControllers)
        {
            float distance = Vector3.Distance(transform.position, health.transform.position);
            if (health == Target)
                Gizmos.color = Color.green;
            else if (distance < RangeAttribute.Value)
                Gizmos.color = Color.yellow;
            else
                Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, health.transform.position);
        }

        if (shootCooldownTimer != null)
            IterationToolkit.Utilities.DrawLabel(transform.position + new Vector3(0, 1.5f, 0), shootCooldownTimer.TimeElapsed.ToString("F2"), Color.white);
    }

    public bool TryModifyAttribute(ScriptableAttribute attributeToAdd, float valueToAdd)
    {
        if (attributeToAdd is ScriptableFloatAttribute floatAtr)
            return (TryModifyAttribute(floatAtr, valueToAdd));
        else
            return (false);
    }

    public bool TryModifyAttribute(ScriptableFloatAttribute attributeToAdd, float valueToAdd)
    {
        foreach (ScriptableAttribute turretAttribute in AllAttributes)
            if (turretAttribute.Compare(attributeToAdd) && turretAttribute is ScriptableFloatAttribute floatAtr)
            {
                floatAtr.AddModifier(valueToAdd);
                OnAttributeModified(turretAttribute, valueToAdd);
                return (true);
            }

        return (false);
    }

    private void OnAttributeModified(ScriptableAttribute attributeModified, float modifierAdded)
    {
        AudioManager.PlayAudio(turretData.OnUpgradeAudioPreset, primaryAudioSource);
    }
}
