using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableTurret", menuName = "TD-GJ/ScriptableTurret", order = 1)]
public class ScriptableTurret : ScriptableItem
{
    [field: SerializeField] public ScriptableProjectile Projectile { get; private set; }
    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue FireRateAttribute { get; private set; }
    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue ShootSpeedAttribute { get; private set; }
    [field: SerializeField] public ScriptableFloatAttributeWithDefaultValue RangeAttribute { get; private set; }
    [field: SerializeField] public ScriptableIntAttributeWithDefaultValue DamageAttribute { get; private set; }

    protected override ItemBehaviour SpawnPrefabSetup(ItemBehaviour newBehaviour)
    {
        return base.SpawnPrefabSetup(newBehaviour);
    }
}
