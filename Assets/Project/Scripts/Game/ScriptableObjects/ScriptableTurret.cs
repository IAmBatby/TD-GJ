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

    [field: SerializeField] public TargetType TargetType { get; private set; }

    [field: Header("Turret Audio Values"), Space(15)]
    [field: SerializeField] public AudioPreset OnShootAudio { get; private set; }
    [field: SerializeField] public AudioPreset OnNewTargetAudio { get; private set; }
    [field: SerializeField] public AudioPreset OnCooldownEndedAudio { get; private set; }
    [field: SerializeField] public AudioPreset OnUpgradeAudio { get; private set; }
}
