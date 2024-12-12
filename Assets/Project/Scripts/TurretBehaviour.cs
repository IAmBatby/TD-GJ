using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehaviour : ItemBehaviour
{
    [field: SerializeField] public ScriptableProjectile Projectile { get; private set; }

    public List<ScriptableAttribute> AllAttributes = new List<ScriptableAttribute>();

    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue FireRateAttribute { get; private set; }
    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue ShotSpeedAttribute { get; private set; }
    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue RangeAttribute { get; private set; }
    [field: SerializeField] public ScriptableIntAttributeWithDefaultValue DamageAttribute { get; private set; }

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
    }

    private void OnMouseEnter() => OnMouseoverToggle.Invoke(true);
    private void OnMouseExit() => OnMouseoverToggle.Invoke(false);

    private void Update()
    {
        if (IsBeingHeld) return;

        UpdateTargets();

        foreach (ShootPosition shootPosition in shootPositions)
            shootPosition.UpdateShootRenderer(Target);

        if (Target != null)
            transform.LookAt(Target.GetTransform().position);

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

        EnemyAI closestEnemy = null;
        float closestDistance = Mathf.Infinity;
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
            }
        }

        Target = closestEnemy;
    }


    private void OnDrawGizmosSelected()
    {
        IterationToolkit.Utilities.DrawCircle(transform.position, RangeAttribute.Value, Color.yellow);
    }
}
