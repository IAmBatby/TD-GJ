using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialCache
{
    private Dictionary<Renderer, List<Material>> rendererDict = new Dictionary<Renderer, List<Material>>();

    public MaterialCache(List<Renderer> renderer, Material newMaterial, Color overrideColor)
    {
        foreach (Renderer r in new List<Renderer>(renderer))
            if (r is LineRenderer)
                renderer.Remove(r);
        newMaterial.color = overrideColor;
        newMaterial.SetColor("_EmissionColor", overrideColor);
        CacheExistingMaterialSetup(renderer);
        ApplyMaterial(renderer, newMaterial);
    }

    public void CacheExistingMaterialSetup(List<Renderer> renderers)
    {
        foreach (Renderer renderer in renderers)
        {
            List<Material> materials = new List<Material>();
            renderer.GetMaterials(materials);
            rendererDict.Add(renderer, materials);
        }
    }

    public void ApplyMaterial(List<Renderer> renderers, Material material)
    {
        foreach (Renderer renderer in renderers)
        {
            List<Material> newList = new List<Material>();
            for (int i = 0; i < renderer.materials.Length; i++)
                newList.Add(material);
            renderer.SetMaterials(newList);
        }
    }

    public void RevertRenderers()
    {
        foreach (KeyValuePair<Renderer, List<Material>> cache in rendererDict)
            if (cache.Key != null)
                cache.Key.SetMaterials(cache.Value);
    }
}
