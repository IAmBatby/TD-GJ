using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType { Closest, Furthest, First, Last }
public class TurretBehaviour : ItemBehaviour
{
    [field: SerializeField] public ScriptableProjectile Projectile { get; private set; }

    public List<ScriptableAttribute> AllAttributes = new List<ScriptableAttribute>();

    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue FireRateAttribute { get; private set; }
    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue ShotSpeedAttribute { get; private set; }
    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue RangeAttribute { get; private set; }
    [field: SerializeField] public ScriptableIntAttributeWithDefaultValue DamageAttribute { get; private set; }

    public TargetType SelectedTargetType { get; private set; }
    //[SerializeField] private Transform shootPosition;
    [SerializeField] private LayerMask shootMask;
    [SerializeField] private List<ShootPosition> shootPositions;

    private IHittable Target;
    private List<IHittable> AllEnemiesInRange = new List<IHittable>();

    private Timer shootCooldownTimer;
    private bool canShoot;


    public ExtendedEvent<bool> OnMouseoverToggle = new ExtendedEvent<bool>();

    protected override void OnSpawn()
    {
        canShoot = true;
        ScriptableTurret turret = ItemData as ScriptableTurret;
        Projectile = turret.Projectile;
        FireRateAttribute.ApplyWithReference(turret.FireRateAttribute);
        ShotSpeedAttribute.ApplyWithReference(turret.ShootSpeedAttribute);
        RangeAttribute.ApplyWithReference(turret.RangeAttribute);
        DamageAttribute.ApplyWithReference(turret.DamageAttribute);
        AllAttributes = new List<ScriptableAttribute>() { FireRateAttribute.Attribute, ShotSpeedAttribute.Attribute, RangeAttribute.Attribute, DamageAttribute.Attribute };
        SelectedTargetType = turret.TargetType;
    }

    private void OnMouseEnter() => OnMouseoverToggle.Invoke(true);
    private void OnMouseExit() => OnMouseoverToggle.Invoke(false);

    private void Update()
    {
        if (IsBeingHeld) return;

        IHittable previousTarget = Target;

        UpdateTargets();

        if (Target != null && previousTarget == null)
        {
            shootCooldownTimer = null;
            canShoot = true;
        }

        foreach (ShootPosition shootPosition in shootPositions)
            shootPosition.UpdateShootRenderer(Target);

        if (Target != null)
        {
            transform.LookAt(Target.GetTransform().position);
        }

        Shoot();
    }

    private void Shoot()
    {
        if (Target == null || canShoot == false) return;

        foreach (ShootPosition shootPosition in shootPositions)
            if (Physics.Raycast(shootPosition.transform.position, Target.GetTransform().position, Mathf.Infinity, shootMask))
                Projectile.SpawnProjectile(shootPosition.transform.position, Target.GetTransform().position, ShotSpeedAttribute.Value, DamageAttribute.Value);

        canShoot = false;
        shootCooldownTimer = new Timer();
        shootCooldownTimer.onTimerEnd.AddListener(DisableCooldown);
        shootCooldownTimer.StartTimer(this, FireRateAttribute.Value);

    }

    private void DisableCooldown() => canShoot = true;



    private void UpdateTargets()
    {
        AllEnemiesInRange.Clear();

        EnemyAI furthestEnemy = null;
        EnemyAI closestEnemy = null;
        EnemyAI firstEnemy = null;
        EnemyAI lastEnemy = null;

        float furthestDistance = Mathf.NegativeInfinity;
        float closestDistance = Mathf.Infinity;
        float closestDestinationRemaining = Mathf.Infinity;
        float furthestDestinationRemaining = Mathf.Infinity;

        foreach (EnemyAI enemy in GameManager.Instance.AllSpawnedEnemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < RangeAttribute.Value)
            {
                AllEnemiesInRange.Add(enemy);
                if (closestEnemy == null || distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }

                if (furthestEnemy == null || distance < furthestDistance)
                {
                    furthestDistance = distance;
                    closestEnemy = enemy;
                }

                if (firstEnemy == null || enemy.Agent.remainingDistance < closestDestinationRemaining)
                {
                    firstEnemy = enemy;
                    closestDestinationRemaining = enemy.Agent.remainingDistance;
                }

                if (lastEnemy == null || enemy.Agent.remainingDistance > furthestDestinationRemaining)
                {
                    lastEnemy = enemy;
                    furthestDestinationRemaining = enemy.Agent.remainingDistance;
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
    }


    private void OnDrawGizmosSelected()
    {
        IterationToolkit.Utilities.DrawCircle(transform.position, RangeAttribute.Value, Color.yellow);
    }
}
