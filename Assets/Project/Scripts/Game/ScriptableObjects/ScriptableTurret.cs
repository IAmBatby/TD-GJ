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
    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue FireRateAttribute { get; private set; }
    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue ShotSpeedAttribute { get; private set; }
    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue RangeAttribute { get; private set; }
    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue DamageAttribute { get; private set; }
    [field: SerializeField] public TargetType TargetType { get; private set; }


    [field: Header("Turret Audio Values"), Space(15)]
    [field: SerializeField] public AudioPreset OnShootAudio { get; private set; }
    [field: SerializeField] public AudioPreset OnNewTargetAudio { get; private set; }
    [field: SerializeField] public AudioPreset OnCooldownEndedAudio { get; private set; }
    [field: SerializeField] public AudioPreset OnUpgradeAudio { get; private set; }
}
