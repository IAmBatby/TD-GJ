using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretModuleBehaviour : ContentBehaviour
{
    [field: SerializeField] public List<ShootPosition> ShootPositions { get; private set; } = new List<ShootPosition>();

    public ScriptableTurretModule ModuleData { get; private set; }

    protected override void OnSpawn()
    {
        base.OnSpawn();

        if (ContentData is ScriptableTurretModule moduleData)
            ModuleData = moduleData;
    }
}
