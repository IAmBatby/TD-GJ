using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalBehaviour : BreakableBehaviour
{
    [SerializeField] private Animator animator;
    protected override void OnPickup()
    {
        animator.enabled = false;
    }
}
