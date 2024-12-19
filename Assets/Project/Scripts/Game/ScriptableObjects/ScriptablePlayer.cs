using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptablePlayer", menuName = "TD-GJ/ScriptableContents/ScriptableHurtables/ScriptablePlayer", order = 1)]
public class ScriptablePlayer : ScriptableHurtable
{
    [field: SerializeField] public ScriptableSkin DefaultSkin { get; private set; }

    public override string GetCategoryName() => "Player";
}
