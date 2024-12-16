using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorManifest", menuName = "TD-GJ/Manifests/ColorManifest", order = 1)]
public class ColorManifest : ScriptableObject
{
    [field: SerializeField] public Color White { get; private set; }
    [field: SerializeField] public Color Black { get; private set; }
    [field: SerializeField] public Color Red { get; private set; }
    [field: SerializeField] public Color Blue { get; private set; }
    [field: SerializeField] public Color Green { get; private set; }
    [field: SerializeField] public Color Yellow { get; private set; }
    [field: SerializeField] public Color Pink { get; private set; }
    [field: SerializeField] public Color Purple { get; private set; }
    [field: SerializeField] public Color Orange { get; private set; }
    [field: SerializeField] public Color Brown { get; private set; }
}
