using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CrystalRockBehaviour : HurtableBehaviour
{
    [SerializeField] private List<ContentSpawner> randomSpawners = new List<ContentSpawner>();
    [SerializeField] private List<ContentSpawner> activeSpawners = new List<ContentSpawner>();
    [SerializeField] private List<ItemBehaviour> spawnedCrystals = new List<ItemBehaviour>();
    private List<ContentDisplayInfo> fakeCrystalInfos = new List<ContentDisplayInfo>();
    private ContentDisplayListing fakeCrystalListing;

    [SerializeField] private int numberOfCrystalsToSpawn;
    [SerializeField] private int PercentChanceToSpawn;
    //I think maybe you should actually animate the item spawner positions and spawn a crystal at the end of the animation
    //Instead of trying to animate the crystal itself
    [SerializeField] private Animator rockSpawnAnimator;

    [SerializeField] private ScriptableContent mineralToSpawn;

    protected override void OnSpawn()
    {
        base.OnSpawn();

        foreach (ContentSpawner spawner in randomSpawners)
            spawner.SetContent(mineralToSpawn);

        GameManager.OnNewWave.AddListener(TrySpawnCrystal);
        OnHealthModified.AddListener(OnDamgeTaken);
        ModifyHealth(-MaxHealth);
        InitializeHighlightInfo();
    }

    private void InitializeHighlightInfo()
    {
        foreach (ContentSpawner spawner in randomSpawners)
        {
            ContentDisplayInfo info = mineralToSpawn.CreateGeneralDisplayInfo();
            info.DisplayMode = DisplayType.Mini;
            info.DisplayColor = GeneralDisplayInfo.DisplayColor;
            info.SetDisplayValues(string.Empty);
            info.PresentMode = PresentationType.Percentage;
            fakeCrystalInfos.Add(info);
        }
        fakeCrystalListing = new ContentDisplayListing(fakeCrystalInfos);
        AddContentDisplayListing(fakeCrystalListing);
        foreach (ContentDisplayInfo info in fakeCrystalInfos)
            fakeCrystalListing.RemoveContentDisplayInfo(info);

        //GeneralDisplayListing.RemoveContentDisplayInfo(HealthDisplayInfo);
    }

    //New wave, roll to spawn droppable Crystal
    private void TrySpawnCrystal()
    {
        if (numberOfCrystalsToSpawn <= 0) return;

        for (int i = numberOfCrystalsToSpawn; i > 0; i--)
        {
            if (activeSpawners.Count >= randomSpawners.Count) break;

            int RandomChance = Random.Range(1, 100);
            Debug.Log("Rolled " + RandomChance.ToString() + "/ 100, Succeed Threshold: " + PercentChanceToSpawn.ToString() + "/100");
            if (RandomChance < PercentChanceToSpawn) return;

            int RandomNumber = Random.Range(0, randomSpawners.Count);

            ContentSpawner randomSpawner = randomSpawners[RandomNumber];
            randomSpawner.gameObject.SetActive(true); //fake crystal under this game object

            activeSpawners.Add(randomSpawner);

            //fakeCrystalInfos[activeSpawners.Count - 1].DisplayColor = Color.grey;
            fakeCrystalInfos[activeSpawners.Count - 1].SetProgressValues(0, 100);
            fakeCrystalListing.AddContentDisplayInfo(fakeCrystalInfos[activeSpawners.Count - 1]);

        }

        if(activeSpawners.Count != 0)
        ResetHealth();
    }

    private void OnDamgeTaken((int, int) health)
    {
        if (health.Item1 <= health.Item2) return;
        if (activeSpawners.Count == 0) return;
        ContentDisplayInfo nextCrystalInfo = fakeCrystalInfos[activeSpawners.Count - 1];
        nextCrystalInfo.SetProgressValues(Health, 0, MaxHealth);
    }

    protected override void OnDeath()
    {
        if(activeSpawners.Count == 0) return;
        int RandomNumber = Random.Range(0, activeSpawners.Count);
        ContentSpawner randomSpawner = activeSpawners[RandomNumber];

        //do animation

        ContentBehaviour spawnedCrystal = randomSpawner.Spawn();
        if (spawnedCrystal is ItemBehaviour crystalBehaviour)
        {
            spawnedCrystals.Add(crystalBehaviour);
            crystalBehaviour.HighlightingDisabled = true;
            crystalBehaviour.OverrideCollisions(true);
            crystalBehaviour.OnItemPickup.AddListener(OnSpawnedCrystalRemoved);
            crystalBehaviour.OnDespawn.AddListener(OnSpawnedCrystalRemoved);
        }
        fakeCrystalInfos[0].DisplayColor = mineralToSpawn.GetDisplayColor();

        randomSpawner.gameObject.SetActive(false);
    }

    private void OnSpawnedCrystalRemoved()
    {
        fakeCrystalListing.RemoveContentDisplayInfo(fakeCrystalInfos[activeSpawners.Count - 1]);
        activeSpawners.Remove(activeSpawners.First());
        ItemBehaviour crystal = spawnedCrystals.First();
        spawnedCrystals.Remove(crystal);
        //crystal.HighlightingDisabled = false;
        crystal.OverrideCollisions(false);
        crystal.OnDespawn.RemoveListener(OnSpawnedCrystalRemoved);
        crystal.OnItemPickup.RemoveListener(OnSpawnedCrystalRemoved);
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
