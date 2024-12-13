using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopSpawner : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemSpawner itemSpawner;
    [SerializeField] private ItemSpawner realItemSpawner;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Material previewMaterial;

    [Header("Values To Care About")]
    [SerializeField] private ScriptableItem itemToSpawn;
    [SerializeField] private int cost;
    [SerializeField, Range(0f,1f)] private float itemModelScaleMultiplier;

    private GameObject fakeItemObject;

    private bool hasBeenPurchased;

    private void Start()
    {
        ItemBehaviour fakeItem = itemSpawner.Spawn(itemToSpawn);
        fakeItemObject = fakeItem.gameObject;
        fakeItemObject.transform.position = itemSpawner.transform.position;
        foreach (MonoBehaviour monoBehavour in fakeItem.GetComponentsInChildren<MonoBehaviour>())
            monoBehavour.enabled = false;

        foreach (MeshRenderer renderer in fakeItem.GetComponentsInChildren<MeshRenderer>())
        {
            Debug.Log("Found Renderer: " + renderer.name);
            List<Material> newMaterials = new List<Material>();
            for (int i = 0; i < renderer.materials.Length; i++)
                newMaterials.Add(previewMaterial);
            renderer.SetMaterials(newMaterials);
        }

        priceText.SetText("$" + cost);


        foreach (Transform fakeTransform in fakeItem.GetComponentsInChildren<Transform>())
            fakeTransform.gameObject.layer = LayerMask.NameToLayer("CursorTarget");

        fakeItemObject.transform.localScale *= itemModelScaleMultiplier;

        GameObject.Destroy(fakeItem);
    }

    public bool TryInteract()
    {
        if (hasBeenPurchased) return (false);
        if (GameManager.Instance.Currency >= cost)
        {
            GameManager.ModifyCurrency(-cost);
            fakeItemObject.SetActive(false);
            ItemBehaviour realItem = itemSpawner.Spawn(itemToSpawn);
            //realItem.transform.position = itemSpawner.transform.position;
            priceText.enabled = false;
            hasBeenPurchased = true;

            return (true);
        }
        return (false);
    }

    private void Update()
    {
        //fakeItemObject.transform.LookAt(GameManager.Player.ActiveCamera.transform.position);
    }
}
