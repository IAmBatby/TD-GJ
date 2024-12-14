using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretModuleBehaviour : ContentBehaviour
{
    [field: SerializeField] public List<ShootPosition> ShootPositions { get; private set; } = new List<ShootPosition>();
}
