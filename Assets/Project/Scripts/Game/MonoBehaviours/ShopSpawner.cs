using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopSpawner : MonoBehaviour, IInteractable, IHighlightable
{
    [Header("Required References")]
    [SerializeField] private ContentSpawner contentSpawner;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Material previewMaterial;

    [SerializeField] private AudioPreset OnPurchaseAudio;

    [Header("Shop Values"), Space(15)]
    [SerializeField] private ScriptableContent contentToSpawn;
    [SerializeField] private int cost;
    [SerializeField] private int turnsUntilRespawn;

    [Header("Shop Content Preview Values"), Space(15)]
    [SerializeField, Range(0f,1f)] private float itemModelScaleMultiplier;
    [SerializeField] private Vector3 itemModelRotation;

    private MaterialController materialController;

    AudioPlayer audioPlayer;
    private GameObject fakeItemObject;
    private bool hasBeenPurchased;
    private int respawnTurnCount;

    private ContentDisplayInfo shopDisplayInfo;
    private List<ContentDisplayInfo> displayInfos;

    private ContentBehaviour realItemSpawn;

    private void OnMouseEnter() => GameManager.Instance.OnContentBehaviourMousedEnter(this);
    private void OnMouseExit() => GameManager.Instance.OnContentBehaviourMousedExit(this);


    private void Awake()
    {
        GameManager.OnWaveFinished.AddListener(OnWaveFinished);
        GameManager.OnGameManagerStart.AddListener(InitializeShop);
        audioPlayer = AudioPlayer.Create(this);
        materialController = new MaterialController();
    }

    private void InitializeShop()
    {
        ContentBehaviour fakeItem = contentSpawner.Spawn(contentToSpawn);
        if (fakeItem == null)
        {
            Debug.LogError("Failed To Spawn Content!");
            return;

        }
        fakeItemObject = fakeItem.gameObject;
        fakeItemObject.transform.position = contentSpawner.transform.position;
        fakeItemObject.transform.rotation = Quaternion.Euler(itemModelRotation);
        foreach (MonoBehaviour monoBehavour in fakeItem.GetComponentsInChildren<MonoBehaviour>())
            monoBehavour.enabled = false;

        foreach (MeshRenderer renderer in fakeItem.GetComponentsInChildren<MeshRenderer>())
        {
            List<Material> newMaterials = new List<Material>();
            for (int i = 0; i < renderer.materials.Length; i++)
                newMaterials.Add(previewMaterial);
            renderer.SetMaterials(newMaterials);
        }

        foreach (SpriteRenderer spriteRenderer in fakeItem.GetComponentsInChildren<SpriteRenderer>())
            spriteRenderer.color = new Color(previewMaterial.color.r, previewMaterial.color.g, previewMaterial.color.b, 0.05f);

        priceText.SetText("$" + cost);

        //Awful
        foreach (Transform fakeTransform in fakeItem.GetComponentsInChildren<Transform>())
            fakeTransform.gameObject.layer = LayerMask.NameToLayer("CursorTarget");

        fakeItemObject.transform.localScale *= itemModelScaleMultiplier;

        GameManager.UnregisterContentBehaviour(fakeItem, false);

        ContentDisplayInfo generalInfo = new ContentDisplayInfo("Item Shop", displayColor: GlobalData.Colors.Yellow);
        shopDisplayInfo = new ContentDisplayInfo("For Sale: " + fakeItem.ContentData.GetDisplayName(), "$" + cost.ToString(), GlobalData.Colors.Yellow, displayIcon: GlobalData.Icons.Currency);
        displayInfos = new List<ContentDisplayInfo>() { generalInfo, shopDisplayInfo };

        List<ContentDisplayListing> fakeListings = fakeItem.GetDisplayListings();
        fakeListings.RemoveAt(0); //To remove the item's name since we say it in our item name
                                  //foreach (ContentDisplayListing displayInfo in fakeListings)
                                  //displayInfos.Add(new ContentDisplayInfo(displayInfo.PrimaryText, displayIcon: displayInfo.DisplayIcon, displayColor: GlobalData.Colors.Yellow));



        GameObject.Destroy(fakeItem);
    }

    public bool TryInteract()
    {
        if (hasBeenPurchased) return (false);
        if (GameManager.Instance.Currency >= cost)
        {
            GameManager.ModifyCurrency(-cost);
            realItemSpawn = contentSpawner.Spawn(contentToSpawn);
            audioPlayer.PlayAudio(OnPurchaseAudio);
            fakeItemObject.SetActive(false);
            priceText.enabled = false;
            hasBeenPurchased = true;
            GameManager.Player.OnItemPickup.AddListener(OnPlayerPickup);


            return (true);
        }
        return (false);
    }

    private void OnPlayerPickup(ItemBehaviour item)
    {
        if (item == realItemSpawn)
        {
            if (turnsUntilRespawn == 0)
                ResetShop();
            GameManager.Player.OnItemPickup.RemoveListener(OnPlayerPickup);
        }
    }

    private void OnWaveFinished()
    {
        if (hasBeenPurchased && realItemSpawn)
        {
            respawnTurnCount++;
            if (respawnTurnCount == turnsUntilRespawn)
            {
                ResetShop();
            }
        }
    }

    private void ResetShop()
    {
        hasBeenPurchased = false;
        fakeItemObject.SetActive(true);
        priceText.enabled = true;
        respawnTurnCount = 0;
        realItemSpawn = null;
    }

    public bool IsHighlightable() => true;
    public Texture2D GetCursor() => null;
    public List<ContentDisplayInfo> GetDisplayInfos() => displayInfos;
    public bool Compare(GameObject go) => (go == gameObject);
    public List<Renderer> GetRenderers() => new List<Renderer>();
    public Color GetColor() => Color.white;
    public MaterialController GetMaterialController() => materialController;

    public List<ContentDisplayListing> GetDisplayListings()
    {
        List<ContentDisplayListing> returnList = new List<ContentDisplayListing>();
        foreach (ContentDisplayInfo contentDisplayInfo in displayInfos)
            returnList.Add(new ContentDisplayListing(contentDisplayInfo));
        return (returnList);
    }
}
