using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalRockBehaviour : HurtableBehaviour
{
    [SerializeField] private List<ContentSpawner> randomSpawners = new List<ContentSpawner>();
    [SerializeField] private List<ContentSpawner> activeSpawners = new List<ContentSpawner>();
    [SerializeField] private List<ItemBehaviour> spawnedCrystals = new List<ItemBehaviour>();

    [SerializeField] private int numberOfCrystalsToSpawn;
    //I think maybe you should actually animate the item spawner positions and spawn a crystal at the end of the animation
    //Instead of trying to animate the crystal itself
    [SerializeField] private Animator rockSpawnAnimator;

    protected override void OnSpawn()
    {
        base.OnSpawn();

        GameManager.OnNewWave.AddListener(TrySpawnCrystal);
        ModifyHealth(-MaxHealth);
    }

    //New wave, roll to spawn droppable Crystal
    private void TrySpawnCrystal()
    {
        if (numberOfCrystalsToSpawn <= 0) return;

        for (int i = numberOfCrystalsToSpawn; i > 0; i--)
        {
            if (activeSpawners.Count >= randomSpawners.Count) break;
            int RandomNumber = Random.Range(0, randomSpawners.Count);

            ContentSpawner randomSpawner = randomSpawners[RandomNumber];
            randomSpawner.gameObject.SetActive(true); //fake crystal under this game object

            activeSpawners.Add(randomSpawner);
        }

        if(activeSpawners.Count != 0)
        ResetHealth();
    }

    protected override void OnDeath()
    {
        if(activeSpawners.Count == 0) return;
        int RandomNumber = Random.Range(0, activeSpawners.Count);
        ContentSpawner randomSpawner = activeSpawners[RandomNumber];

        //do animation

        randomSpawner.Spawn();

        randomSpawner.gameObject.SetActive(false);
        activeSpawners.Remove(randomSpawner);

        if (activeSpawners.Count != 0)
            ResetHealth();
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
