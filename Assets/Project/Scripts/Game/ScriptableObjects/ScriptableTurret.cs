using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableTurret", menuName = "TD-GJ/ScriptableContents/ScriptableItems/ScriptableTurret", order = 1)]
public class ScriptableTurret : ScriptableItem
{
    [field: Header("Turret Values"), Space(15)]
    [field: SerializeField] public ScriptableProjectile Projectile { get; private set; }
    [field: SerializeField] public List<ScriptableTurretModule> Modules { get; private set; }

    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public float Range { get; private set; }
    [field: SerializeField] public float ShotSpeed { get; private set; }
    [field: SerializeField] public float FireRate { get; private set; }
    [field: SerializeField, Range(0f,1f)] public float Accuracy { get; private set; }

    [field: SerializeField] public Vector3 AimOffset { get; private set; }
    [field: SerializeField] public TargetType TargetType { get; private set; }

    [field: Header("Turret Visual Values"), Space(15)]
    [field: SerializeField] public ReactionInfo OnShootReaction { get; private set; }
    [field: SerializeField] public ReactionInfo OnNewTargetReaction { get; private set; }
    [field: SerializeField] public ReactionInfo OnCooldownEndedReaction { get; private set; }
    [field: SerializeField] public ReactionInfo OnUpgradedReaction { get; private set; }

    [field: SerializeField] public float DeploymentShootCooldown { get; private set; }

    public override string GetCategoryName() => "Turret Base";
}
