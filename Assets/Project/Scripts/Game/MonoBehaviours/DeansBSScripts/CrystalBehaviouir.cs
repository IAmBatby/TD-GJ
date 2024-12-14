using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalBehaviouir : ItemBehaviour
{
    public Animator anim;
    protected override void OnPickup()
    {
        anim.enabled = false;
    }

}
