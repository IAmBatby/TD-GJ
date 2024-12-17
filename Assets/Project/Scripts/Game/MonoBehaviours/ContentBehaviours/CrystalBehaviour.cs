using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalBehaviour : ItemBehaviour
{
    public void ForwardedTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out TurretBaseBehaviour turret))
        {
            if (turret.TryUpgradeTurretModule())
            {

                if (GameManager.Player.ActiveItem == this)
                    GameManager.Player.DropItem(transform.position);
                GameManager.UnregisterContentBehaviour(this, true);
            }
        }
    }
    public override void RegisterBehaviour()
    {
        ContentManager.RegisterBehaviour(this);
        base.RegisterBehaviour();
    }
    public override void UnregisterBehaviour(bool destroyOnUnregistration)
    {
        ContentManager.UnregisterBehaviour(this, destroyOnUnregistration);
        base.UnregisterBehaviour(destroyOnUnregistration);
    }

}
