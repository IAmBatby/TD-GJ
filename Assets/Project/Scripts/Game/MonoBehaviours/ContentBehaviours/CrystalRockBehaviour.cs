using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalRockBehaviour : HurtableBehaviour
{
    [SerializeField] private List<ContentSpawner> randomSpawners = new List<ContentSpawner>();
    [SerializeField] private List<ItemBehaviour> spawnedCrystals = new List<ItemBehaviour>();

    //I think maybe you should actually animate the item spawner positions and spawn a crystal at the end of the animation
    //Instead of trying to animate the crystal itself
    [SerializeField] private Animator rockSpawnAnimator;

    protected override void OnSpawn()
    {
        base.OnSpawn();

        GameManager.OnNewWave.AddListener(TrySpawnCrystal);
        OnHealthModified.AddListener(TryDropCrystal);
    }

    //Damage taken, "release" spawned Crystal
    private void TryDropCrystal((int oldHealth, int newHealth) modifiedHealth)
    {

    }


    //New wave, roll to spawn droppable Crystal
    private void TrySpawnCrystal()
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
