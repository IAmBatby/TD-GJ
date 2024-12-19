using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static float GetScaledValue(float value, float scaleRate, int waveCount)
    {
        //Debug.Log("Setting New Value to: " + (value + (waveCount * (scaleRate * value / 100))));
        return (value + (waveCount * (scaleRate * value / 100)));
    }

    public static Vector2 GetScaledValue(Vector2 value, float scaleRate, int waveCount)
    {
        return new Vector2(GetScaledValue(value.x, scaleRate, waveCount), GetScaledValue(value.y, scaleRate, waveCount));
    }

    public static void DrawPrefabPreview(Transform context, GameObject prefab, Color primary, Color secondary, ref List<MeshFilter> filters)
    {
        Matrix4x4 prevMatrix = Gizmos.matrix;
        Color prevColor = Gizmos.color;
        Gizmos.matrix = context.localToWorldMatrix;

        if (prefab == null) return;
        if (filters == null)
            filters = new List<MeshFilter>();
        if (filters.Count == 0)
            foreach (MeshFilter renderer in prefab.GetComponentsInChildren<MeshFilter>())
                filters.Add(renderer);

        for (int i = 0; i < filters.Count; i++)
            if (filters[i] == null || filters[i].transform.root != prefab.transform)
            {
                filters.Clear();
                return;
            }

        Gizmos.color = new Color(primary.r, primary.g, primary.b, 0.3f);
        foreach (MeshFilter renderer in filters)
            Gizmos.DrawMesh(renderer.sharedMesh, renderer.transform.position, renderer.transform.rotation, renderer.transform.lossyScale);
        Gizmos.color = new Color(secondary.r, secondary.g, secondary.b, 0.05f);
        foreach (MeshFilter renderer in filters)
            Gizmos.DrawWireMesh(renderer.sharedMesh, renderer.transform.position, renderer.transform.rotation, renderer.transform.lossyScale);

        Gizmos.color = prevColor;
        Gizmos.matrix = prevMatrix;
    }
}
