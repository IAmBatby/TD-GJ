using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialController
{
    private Dictionary<Renderer, List<Material>> cachedSharedMaterialDict = new Dictionary<Renderer, List<Material>>();
    private List<Renderer> allRenderers = new List<Renderer>();

    private Timer revertTimer;

    public MaterialController(List<Renderer> renderers = null)
    {
        if (renderers == null) return;
        foreach (Renderer renderer in renderers)
            AddNewRenderer(renderer);
    }

    public void AddNewRenderer(Renderer renderer)
    {
        if (cachedSharedMaterialDict.ContainsKey(renderer)) return;
        if (renderer is LineRenderer) return;
        List<Material> sharedMats = new List<Material>();
        renderer.GetSharedMaterials(sharedMats);
        cachedSharedMaterialDict.Add(renderer, sharedMats);
        allRenderers.Add(renderer);
    }

    public void RemoveRenderer(Renderer renderer)
    {
        if (allRenderers.Contains(renderer))
            allRenderers.Remove(renderer);
        if (cachedSharedMaterialDict.ContainsKey(renderer))
            cachedSharedMaterialDict.Remove(renderer);
    }

    public void ApplyMaterial(Material material, Color overrideColor, float revertTime = 0f)
    {
        material.color = overrideColor;
        material.SetColor("_EmissionColor", overrideColor);
        ApplyMaterial(material, revertTime);
    }

    public void ApplyMaterial(Material material, float revertTime = 0f)
    {
        List<Material> newList = new List<Material>();
        foreach (Renderer renderer in allRenderers)
        {
            newList.Clear();
            for (int i = 0; i < renderer.materials.Length; i++)
                newList.Add(material);
            renderer.SetMaterials(newList);
        }

        if (revertTime > 0f)
        {
            revertTimer = new Timer();
            revertTimer.onTimerEnd.AddListener(ResetRenderers);
            revertTimer.StartTimer(GlobalData.Instance, revertTime);
        }
    }

    public void ResetRenderers()
    {
        foreach (Renderer renderer in allRenderers)
            ResetRenderer(renderer);
        revertTimer = null;
    }

    public void ResetRenderer(Renderer renderer)
    {
        if (renderer != null && cachedSharedMaterialDict.TryGetValue(renderer, out List<Material> sharedMats))
            renderer.SetMaterials(sharedMats);
    }
}
