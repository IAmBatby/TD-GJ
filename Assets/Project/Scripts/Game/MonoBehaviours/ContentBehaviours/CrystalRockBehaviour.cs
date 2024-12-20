using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CrystalRockBehaviour : HurtableBehaviour
{
    [SerializeField] private List<ContentSpawner> randomSpawners = new List<ContentSpawner>();
    [SerializeField] private List<ContentSpawner> activeSpawners = new List<ContentSpawner>();
    [SerializeField] private List<ItemBehaviour> spawnedCrystals = new List<ItemBehaviour>();
    
    private List<RockSpawnInfo> rockSpawnInfos = new List<RockSpawnInfo>();
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
        fakeCrystalListing = new ContentDisplayListing();
        AddContentDisplayListing(fakeCrystalListing);
        foreach (ContentSpawner spawner in randomSpawners)
        {
            spawner.SetContent(mineralToSpawn);
            rockSpawnInfos.Add(new RockSpawnInfo(this, spawner, fakeCrystalListing));
        }

        GameManager.OnNewWave.AddListener(TryEnableSpawner);
        ModifyHealth(-MaxHealth);
    }

    //New wave, roll to spawn droppable Crystal
    private void TryEnableSpawner()
    {
        if (numberOfCrystalsToSpawn <= 0) return;

        for (int i = numberOfCrystalsToSpawn; i > 0; i--)
        {
            List<RockSpawnInfo> inactiveRocks = rockSpawnInfos.Where(r => r.CurrentState == RockSpawnInfo.SpawnState.Inactive).ToList();
            if (inactiveRocks.Count == 0) return;
            int RandomChance = Random.Range(1, 100);
            Debug.Log("Rolled " + RandomChance.ToString() + "/ 100, Succeed Threshold: " + PercentChanceToSpawn.ToString() + "/100");
            if (RandomChance < PercentChanceToSpawn) return;
            inactiveRocks[Random.Range(0, inactiveRocks.Count)].Activate();
        }

        List<RockSpawnInfo> activeRocks = rockSpawnInfos.Where(r => r.CurrentState == RockSpawnInfo.SpawnState.Active).ToList();
        if (activeRocks.Count == 1)
            ResetHealth();
    }

    protected override void OnDeath()
    {
        List<RockSpawnInfo> activeRocks = rockSpawnInfos.Where(r => r.CurrentState == RockSpawnInfo.SpawnState.Active).ToList();
        if (activeRocks.Count == 0) return;
        activeRocks[Random.Range(0, activeRocks.Count)].Spawn();
    }

    public void OnSpawnedCrystalRemoved()
    {
        List<RockSpawnInfo> activeRocks = rockSpawnInfos.Where(r => r.CurrentState == RockSpawnInfo.SpawnState.Active).ToList();
        if (activeRocks.Count > 0)
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

public class RockSpawnInfo
{
    public CrystalRockBehaviour rockSpawner;
    public ContentSpawner contentSpawner;
    public ItemBehaviour spawnedCrystal;
    public ContentDisplayInfo info;
    public ContentDisplayListing listing;
    public enum SpawnState { Inactive, Active, Spawned }
    public SpawnState CurrentState { get; private set; }

    public RockSpawnInfo(CrystalRockBehaviour newRockSpawner, ContentSpawner newSpawner, ContentDisplayListing newListing)
    {
        contentSpawner = newSpawner;
        listing = newListing;
        info = contentSpawner.DefaultContent.CreateGeneralDisplayInfo();
        rockSpawner = newRockSpawner;
        info.DisplayMode = DisplayType.Mini;
        info.DisplayColor = contentSpawner.DefaultContent.GetDisplayColor();
        info.SetDisplayValues(string.Empty);
        info.PresentMode = PresentationType.Percentage;
    }

    public void Activate()
    {
        CurrentState = SpawnState.Active;
        info.DisplayColor = Color.grey;
        listing.AddContentDisplayInfo(info);
        contentSpawner.gameObject.SetActive(true);
    }

    public void Spawn()
    {
        CurrentState = SpawnState.Spawned;
        ContentBehaviour behaviour = contentSpawner.Spawn();
        if (behaviour is ItemBehaviour item)
            spawnedCrystal = item;

        spawnedCrystal.OnItemPickup.AddListener(Deactivate);
        spawnedCrystal.OnDespawn.AddListener(Deactivate);
        spawnedCrystal.OverrideCollisions(true, false, true);
        info.DisplayColor = spawnedCrystal.ItemData.GetDisplayColor();
        contentSpawner.gameObject.SetActive(false);
    }

    private void Deactivate()
    {
        CurrentState = SpawnState.Inactive;
        spawnedCrystal.OnItemPickup.RemoveListener(Deactivate);
        spawnedCrystal.OnDespawn.RemoveListener(Deactivate);
        spawnedCrystal.OverrideCollisions(false);
        listing.RemoveContentDisplayInfo(info);
        rockSpawner.OnSpawnedCrystalRemoved();
        spawnedCrystal = null;
    }
}
