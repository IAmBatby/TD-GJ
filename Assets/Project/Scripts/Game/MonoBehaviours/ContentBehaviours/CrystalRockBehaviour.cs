using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalRockBehaviour : HurtableBehaviour
{

    //Damage taken, "release" spawned Crystal
    private void DropCrystal()
    {

    }


    //New wave, roll to spawn droppable Crystal
    private void SpawnCrystal()
    {

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
