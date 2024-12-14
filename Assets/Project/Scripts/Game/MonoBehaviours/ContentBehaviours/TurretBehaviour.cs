using IterationToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TargetType { Closest, Furthest, First, Last }
public class TurretBehaviour : ItemBehaviour
{
    [Header("Required References"), Space(15)]
    [SerializeField] private List<ShootPosition> shootPositions;
    [SerializeField] private List<Transform> barrelTransforms = new List<Transform>();
    [SerializeField] private LayerMask shootMask;

    [field: Header("Runtime Values (Don't Touch)"), Space(15)]
    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue FireRateAttribute { get; private set; }
    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue ShotSpeedAttribute { get; private set; }
    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue RangeAttribute { get; private set; }
    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue DamageAttribute { get; private set; }
    [SerializeField] private List<HurtableBehaviour> AllEnemiesInRange = new List<HurtableBehaviour>();
    [SerializeField] private List<HurtableBehaviour> BlacklistedTargets = new List<HurtableBehaviour>();

    public TargetType SelectedTargetType { get; private set; }

    public ScriptableTurret TurretData { get; private set; }
    public ScriptableProjectile Projectile { get; private set; }

    [HideInInspector] public List<ScriptableAttribute> AllAttributes = new List<ScriptableAttribute>();

    private Dictionary<ScriptableAttribute, ContentDisplayInfo> attributeDisplayInfoDict = new Dictionary<ScriptableAttribute, ContentDisplayInfo>();

    private HurtableBehaviour Target;
    private Timer shootCooldownTimer;
    private bool canShoot;

    protected override void OnSpawn()
    {
        base.OnSpawn();
        canShoot = true;
        TurretData = ItemData as ScriptableTurret;
        Projectile = TurretData.Projectile;
        FireRateAttribute.ApplyWithReference(TurretData.FireRateAttribute);
        ShotSpeedAttribute.ApplyWithReference(TurretData.ShotSpeedAttribute);
        RangeAttribute.ApplyWithReference(TurretData.RangeAttribute);
        DamageAttribute.ApplyWithReference(TurretData.DamageAttribute);
        AllAttributes = new List<ScriptableAttribute>() { FireRateAttribute.Attribute, ShotSpeedAttribute.Attribute, RangeAttribute.Attribute, DamageAttribute.Attribute };
        SelectedTargetType = TurretData.TargetType;

        foreach (ScriptableAttribute attribute in AllAttributes)
        {
            ContentDisplayInfo newInfo = new ContentDisplayInfo(attribute.GetDisplayString(), attribute.DisplayIcon, attribute.DisplayColor);
            attributeDisplayInfoDict.Add(attribute, newInfo);
            AddContentDisplayInfo(newInfo);
        }

        BlacklistTarget(GameManager.Player);

    }

    private void BlacklistTarget(HurtableBehaviour hurtable)
    {
        BlacklistedTargets.Add(hurtable);
    }

    private void FixedUpdate()
    {
        //transform.rotation = Quaternion.Euler(0,transform.rotation.eulerAngles.y,0);
    }

    private void Update()
    {
        if (IsBeingHeld) return;

        HurtableBehaviour previousTarget = Target;

        UpdateTargets();

        if (Target != null && previousTarget == null)
        {
            shootCooldownTimer = null;
            canShoot = true;
        }

        if (Target != null && Target != previousTarget)
            AudioPlayer.PlayAudio(TurretData.OnNewTargetAudio);

        foreach (ShootPosition shootPosition in shootPositions)
            shootPosition.UpdateShootRenderer(Target);

        foreach (Transform barrelTransform in barrelTransforms)
        {
            if (Target != null)
                barrelTransform.LookAt(Target.transform.position);
            else
                barrelTransform.rotation = Quaternion.identity;
        }

        if (Target != null)
            transform.LookAt(Target.transform.position);
        else
            transform.rotation = Quaternion.identity;

        Shoot();
    }

    private void Shoot()
    {
        if (Target == null || canShoot == false) return;

        bool didShoot = false;
        /*
        foreach (ShootPosition shootPosition in shootPositions)
            if (Physics.Raycast(shootPosition.transform.position, Target.LinkedBehaviour.transform.position, out RaycastHit hit, Mathf.Infinity, shootMask))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    Projectile.SpawnProjectile(shootPosition.transform.position, Target.LinkedBehaviour.transform.position, ShotSpeedAttribute.Value, Mathf.RoundToInt(DamageAttribute.Value));
                    didShoot = true;
                }
            }
        */

        foreach (ShootPosition shootPosition in shootPositions)
            Projectile.SpawnProjectile(shootPosition.transform.position, Target.transform.position, ShotSpeedAttribute.Value, Mathf.RoundToInt(DamageAttribute.Value));

        didShoot = true;

        if (didShoot == true)
        {
            AudioPlayer.PlayAudio(TurretData.OnShootAudio);
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
        AudioPlayer.PlayAudio(TurretData.OnCooldownEndedAudio);
        canShoot = true;
    }



    private void UpdateTargets()
    {
        AllEnemiesInRange.Clear();

        HurtableBehaviour furthestEnemy = null;
        HurtableBehaviour closestEnemy = null;
        HurtableBehaviour firstEnemy = null;
        HurtableBehaviour lastEnemy = null;

        float furthestDistance = Mathf.NegativeInfinity;
        float closestDistance = Mathf.Infinity;
        float closestDestinationRemaining = Mathf.Infinity;
        float furthestDestinationRemaining = Mathf.Infinity;

        foreach (HurtableBehaviour hurtable in GameManager.Instance.AllHurtables)
        {
            if (BlacklistedTargets.Contains(hurtable)) continue;
            if (hurtable.Health == 0) continue;
            float distance = Vector3.Distance(transform.position, hurtable.transform.position);
            if (distance <= RangeAttribute.Value)
            {
                AllEnemiesInRange.Add(hurtable);
                if (closestEnemy == null || distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = hurtable;
                }

                if (furthestEnemy == null || distance < furthestDistance)
                {
                    furthestDistance = distance;
                    closestEnemy = hurtable;
                }

                if (hurtable is EnemyBehaviour enemy)
                {
                    if (firstEnemy == null || enemy.RemainingDestinationDistance < closestDestinationRemaining)
                    {
                        firstEnemy = hurtable;
                        closestDestinationRemaining = enemy.RemainingDestinationDistance;
                    }

                    if (lastEnemy == null || enemy.RemainingDestinationDistance > furthestDestinationRemaining)
                    {
                        lastEnemy = hurtable;
                        furthestDestinationRemaining = enemy.RemainingDestinationDistance;
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

        foreach (HurtableBehaviour health in GameManager.Instance.AllHurtables)
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
        if (attributeDisplayInfoDict.TryGetValue(attributeModified, out ContentDisplayInfo displayInfo))
            displayInfo.DisplayText = attributeModified.GetDisplayString();
        AudioPlayer.PlayAudio(TurretData.OnUpgradeAudio);
    }
}
