using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public enum TargetType { Closest, Furthest, First, Last }
public class TurretBaseBehaviour : ItemBehaviour
{
    [SerializeField] private ContentSpawner moduleSpawner;
    [SerializeField] private Transform rotator;
    [SerializeField] private LayerMask shootMask;
    [SerializeField] private MeshRenderer rangePreviewRenderer;
    [SerializeField] private LineRenderer rangePreviewLineRenderer;

    [field: Header("Runtime Values (Don't Touch)"), Space(15)]

    [field: SerializeField] public TurretModuleBehaviour ActiveModule { get; private set; }

    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue FireRateAttribute { get; private set; }
    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue ShotSpeedAttribute { get; private set; }
    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue AccuracyAttribute { get; private set; }
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

    public Vector3 aimOffset;

    public Vector3 AimPosition => (transform.position + aimOffset);

    protected override void OnDrop()
    {
        base.OnDrop();
        EnableCooldown(TurretData.DeploymentShootCooldown);
    }
    protected override void OnSpawn()
    {
        base.OnSpawn();
        canShoot = true;
        rangePreviewRenderer.enabled = false;
        TurretData = ItemData as ScriptableTurret;
        Projectile = TurretData.Projectile;
        aimOffset = TurretData.AimOffset;

        DamageAttribute.ApplyWithNewReference(GlobalData.Attributes.DamageAttribute, TurretData.Damage);
        RangeAttribute.ApplyWithNewReference(GlobalData.Attributes.RangeAttribute, TurretData.Range);
        ShotSpeedAttribute.ApplyWithNewReference(GlobalData.Attributes.ShotSpeedAttribute, TurretData.ShotSpeed);
        FireRateAttribute.ApplyWithNewReference(GlobalData.Attributes.FireRateAttribute, TurretData.FireRate);
        AccuracyAttribute.ApplyWithNewReference(GlobalData.Attributes.AccuracyAttribute, TurretData.Accuracy);

        AllAttributes = new List<ScriptableAttribute>() { FireRateAttribute.Attribute, ShotSpeedAttribute.Attribute, RangeAttribute.Attribute, DamageAttribute.Attribute, AccuracyAttribute.Attribute };
        SelectedTargetType = TurretData.TargetType;

        List<ContentDisplayInfo> atrInfos = new List<ContentDisplayInfo>();
        foreach (ScriptableAttribute attribute in AllAttributes)
        {
            if (attribute is ScriptableFloatAttribute floatAtr)
            {
                ContentDisplayInfo newInfo = new ContentDisplayInfo(floatAtr.GetAttributeValue().ToString(), displayIcon: null, displayColor: attribute.DisplayColor);
                newInfo.DisplayMode = DisplayType.Mini;
                attributeDisplayInfoDict.Add(attribute, newInfo);
                atrInfos.Add(newInfo);
            }
        }
        AddContentDisplayListing(new ContentDisplayListing(atrInfos));

        BlacklistTarget(GameManager.Player);

        SetNewModule(TurretData.Modules.First());
    }

    public void SetNewModule(ScriptableTurretModule turretModule)
    {
        TurretModuleBehaviour newModule = turretModule.SpawnPrefab() as TurretModuleBehaviour;
        newModule.transform.SetParent(transform);
        newModule.transform.position = moduleSpawner.transform.position;

        if (ActiveModule != null)
        {
            foreach (Renderer renderer in ActiveModule.Renderers)
                MaterialController.RemoveRenderer(renderer);
            GameManager.UnregisterContentBehaviour(ActiveModule, true);
        }


        ActiveModule = newModule;

        foreach (Renderer renderer in ActiveModule.Renderers)
            MaterialController.AddNewRenderer(renderer);

        
        GeneralDisplayInfo.PresentMode = PresentationType.Progress;
        GeneralDisplayInfo.SetDisplayValues(ActiveModule.ModuleData.GetDisplayName());
        GeneralDisplayInfo.SetProgressValues(TurretData.Modules.IndexOf(turretModule) + 1, TurretData.Modules.Count);
    }

    public bool TryUpgradeTurretModule()
    {
        if (ActiveModule.ModuleData == TurretData.Modules.Last()) return false;

        int i = TurretData.Modules.IndexOf(ActiveModule.ModuleData);

        SetNewModule(TurretData.Modules[i + 1]);

        return true;
    }
    private void Update()
    {
        UpdateRangePreview();
        if (IsBeingHeld == true || ActiveModule == null) return;

        HurtableBehaviour previousTarget = Target;

        UpdateTargets();
        /*
        if (Target != null && previousTarget == null)
        {
            shootCooldownTimer = null;
            canShoot = true;
        }*/

        if (Target != null && Target != previousTarget)
            ReactionPlayer.Play(TurretData.OnNewTargetReaction);

        foreach (ShootPosition shootPosition in ActiveModule.ShootPositions)
        {
            shootPosition.ToggleRenderer(Target != null && IsHighlighted && IsBeingHeld == false);
            shootPosition.UpdateShootRenderer(Target);
        }


        if (Target != null)
        {
            transform.LookAt(Target.transform.position);
            ActiveModule.transform.localEulerAngles = new Vector3(transform.eulerAngles.x, 0, 0);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
        else
        {
            transform.rotation = Quaternion.identity;
            ActiveModule.transform.rotation = Quaternion.identity;
        }

        Shoot();
    }

    private void Shoot()
    {
        if (Target == null || canShoot == false) return;

        foreach (ShootPosition shootPosition in ActiveModule.ShootPositions)
        {
            shootPosition.ShootAtTarget(Projectile, Target, Mathf.RoundToInt(DamageAttribute.Value), ShotSpeedAttribute.Value, AccuracyAttribute.Value);
            //Projectile.SpawnProjectile(shootPosition.transform.position, Target.transform.position, ShotSpeedAttribute.Value, Mathf.RoundToInt(DamageAttribute.Value));
        }

        ReactionPlayer.Play(TurretData.OnShootReaction);
        EnableCooldown(FireRateAttribute.Value);
    }

    private void EnableCooldown(float CooldownLength)
    {
        if (CooldownLength <= 0f) return;
        canShoot = false;
        if (shootCooldownTimer != null)
            shootCooldownTimer.TryStopTimer();
        shootCooldownTimer = new Timer(this, FireRateAttribute.Value, DisableCooldown);
    }

    private void DisableCooldown()
    {
        ReactionPlayer.Play(TurretData.OnCooldownEndedReaction);
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



        foreach (HurtableBehaviour hurtable in ContentManager.GetBehaviours<HurtableBehaviour>())
        {
            if (BlacklistedTargets.Contains(hurtable)) continue;
            if (hurtable.Health == 0) continue;
            float distance = Vector3.Distance(AimPosition, hurtable.transform.position);
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

    private void BlacklistTarget(HurtableBehaviour hurtable) => BlacklistedTargets.Add(hurtable);


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
            displayInfo.SetDisplayValues(attributeModified.GetSecondaryDisplayString());
        ReactionPlayer.Play(TurretData.OnUpgradedReaction);
    }

    private void UpdateRangePreview()
    {
        int circlePoints = 64;

        List<Vector3> returnList = new List<Vector3>();

        foreach (Vector3 circlePos in FindCirclePoints(circlePoints, RangeAttribute.Value))
            returnList.Add(AimPosition + circlePos);
        

        rangePreviewLineRenderer.enabled = IsHighlighted == true && IsBeingHeld == false;
        rangePreviewLineRenderer.positionCount = circlePoints;
        rangePreviewLineRenderer.SetPositions(returnList.ToArray());
    }

    static Vector3[] FindCirclePoints(int pointCount = 64, float radius = 1f)
    {
        Vector3[] points = new Vector3[pointCount];
        Vector3 current = Vector3.up;
        Quaternion rot = Quaternion.Euler(0.0f, 0.0f, 360.0f / pointCount);
        for (int i = 0; i < pointCount; i++)
        {
            points[i] = current;
            current = rot * current;
        }

        for (int i = 0; i < pointCount; i++)
        {
            points[i] = new Vector3(points[i].x * radius, points[i].z * radius, points[i].y * radius);
        }

        return points;
    }

    public override List<Renderer> GetRenderers() => Renderers.Concat(ActiveModule.Renderers).ToList();

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

    private void OnDrawGizmosSelected()
    {
        //Gizmos.matrix = transform.localToWorldMatrix;

        if (Target == null) return;

        foreach (ShootPosition shotPos in ActiveModule.ShootPositions)
        {
            Vector3 size = new Vector3(0.3f, 0.3f, 0.3f);

            Vector3 rushingPosition = Target.transform.position + (Target.transform.forward + new Vector3(5, 0, 0));
            Vector3 draggingPosition = Target.transform.position + (-Target.transform.forward + new Vector3(-5, 0, 0));
            Vector3 realPoint = shotPos.GetAccuracyPosition(Target.transform, AccuracyAttribute.Value);
            Gizmos.DrawLine(transform.position, realPoint);
            Gizmos.DrawWireCube(rushingPosition, size);
            Gizmos.DrawWireCube(draggingPosition, size);
            Gizmos.DrawLine(rushingPosition, draggingPosition);
            Gizmos.color = Color.green;
            Gizmos.DrawCube(realPoint, size);
        }
    }
}
