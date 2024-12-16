using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttributeManifest", menuName = "TD-GJ/Manifests/AttributeManifest", order = 1)]
public class AttributeManifest : ScriptableObject
{
    [field: SerializeField] public ScriptableAttribute DamageAttribute { get; private set; }
    [field: SerializeField] public ScriptableAttribute RangeAttribute {  get; private set; }
    [field: SerializeField] public ScriptableAttribute ShotSpeedAttribute { get; private set; }
    [field: SerializeField] public ScriptableAttribute FireRateAttribute { get; private set; }
}
