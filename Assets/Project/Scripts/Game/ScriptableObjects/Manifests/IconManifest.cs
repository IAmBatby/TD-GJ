using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IconManifest", menuName = "TD-GJ/Manifests/IconManifest", order = 1)]
public class IconManifest : ScriptableObject
{
    [field: SerializeField] public Sprite Health { get; private set; }
    [field: SerializeField] public Sprite Currency { get; private set; }
}
