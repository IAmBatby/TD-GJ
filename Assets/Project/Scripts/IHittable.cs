using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable
{
    public void RecieveHit(int value);

    public Transform GetTransform();
}
