using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITarget
{
    public Vector3 GetTargetPosition(float accuracyRate);
}
