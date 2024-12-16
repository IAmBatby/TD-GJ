using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinManifest", menuName = "TD-GJ/Manifests/SkinManifest", order = 1)]
public class SkinManifest : ScriptableObject
{
    [field: SerializeField] public List<ScriptableSkin> Skins { get; private set; } = new List<ScriptableSkin>();
}
