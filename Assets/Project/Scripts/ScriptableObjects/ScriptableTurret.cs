using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableTurret", menuName = "TD-GJ/ScriptableTurret", order = 1)]
public class ScriptableTurret : ScriptableItem
{
    [field: SerializeField] public ScriptableProjectile Projectile { get; private set; }
    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue FireRateAttribute { get; private set; }
    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue ShotSpeedAttribute { get; private set; }
    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue RangeAttribute { get; private set; }
    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue DamageAttribute { get; private set; }
    [field: SerializeField] public TargetType TargetType { get; private set; }


    [field: Header("Audio"), Space(15)]
    [field: SerializeField] public AudioPreset OnShootAudioPreset { get; private set; }
    [field: SerializeField] public AudioPreset OnNewTargetAudioPreset { get; private set; }
    [field: SerializeField] public AudioPreset OnCooldownEndedAudioPreset { get; private set; }
    [field: SerializeField] public AudioPreset OnUpgradeAudioPreset { get; private set; }

    protected override ItemBehaviour SpawnPrefabSetup(ItemBehaviour newBehaviour)
    {
        return base.SpawnPrefabSetup(newBehaviour);
    }
}
