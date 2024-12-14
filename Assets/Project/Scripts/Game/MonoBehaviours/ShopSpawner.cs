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

    AudioPlayer audioPlayer;
    private GameObject fakeItemObject;
    private bool hasBeenPurchased;
    private int respawnTurnCount;

    private ContentDisplayInfo shopDisplayInfo;
    private List<ContentDisplayInfo> displayInfos;

    private void OnMouseEnter() => GameManager.Instance.OnContentBehaviourMousedEnter(this);
    private void OnMouseExit() => GameManager.Instance.OnContentBehaviourMousedExit(this);


    private void Awake()
    {
        GameManager.Instance.OnWaveFinished.AddListener(OnWaveFinished);
        audioPlayer = AudioPlayer.Create(this);
        shopDisplayInfo = new ContentDisplayInfo("$" + cost.ToString(), null, Color.yellow);
        displayInfos = new List<ContentDisplayInfo>() { shopDisplayInfo };
    }

    private void Start()
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

        foreach (ContentDisplayInfo displayInfo in fakeItem.GetDisplayInfos())
            displayInfos.Add(new ContentDisplayInfo(displayInfo.DisplayText, displayInfo.DisplayIcon, Color.yellow));
    }

    public bool TryInteract()
    {
        if (hasBeenPurchased) return (false);
        if (GameManager.Instance.Currency >= cost)
        {
            GameManager.ModifyCurrency(-cost);
            fakeItemObject.SetActive(false);
            ContentBehaviour realContent = contentSpawner.Spawn(contentToSpawn);
            audioPlayer.PlayAudio(OnPurchaseAudio);
            priceText.enabled = false;
            hasBeenPurchased = true;

            return (true);
        }
        return (false);
    }

    private void OnWaveFinished()
    {
        if (hasBeenPurchased)
        {
            respawnTurnCount++;
            if (respawnTurnCount == turnsUntilRespawn)
            {
                hasBeenPurchased = false;
                fakeItemObject.SetActive(true);
                priceText.enabled = true;
                respawnTurnCount = 0;
            }
        }
    }

    public bool IsHighlightable() => true;
    public Texture2D GetCursor() => null;
    public List<ContentDisplayInfo> GetDisplayInfos() => displayInfos;
}
