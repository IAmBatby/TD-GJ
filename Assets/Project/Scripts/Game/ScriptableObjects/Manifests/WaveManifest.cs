using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "WaveManifest", menuName = "TD-GJ/Manifests/WaveManifest", order = 1)]
public class WaveManifest : ScriptableObject
{
    [field: SerializeField] public List<ScriptableWave> Waves { get; private set; } = new List<ScriptableWave>();
    [SerializeField] private bool renameScriptableWaves;

    public float GetWaveLength(int waveIndex) => waveIndex < Waves.Count ? GetWaveLength(Waves[waveIndex]) : 0f;


    public float GetWaveLength(ScriptableWave wave) => wave.GetFinalTime();

    public int GetAdditiveHealth(int value, int waveIndex)
    {
        int returnValue = value;

        if (waveIndex < Waves.Count)
            for (int i = 0; i < waveIndex + 1; i++)
                returnValue += Waves[i].AdditiveHealth;

        return (returnValue);
    }

    public float GetAdditiveSpeed(float value, int waveIndex)
    {
        float returnValue = value;

        if (waveIndex < Waves.Count)
            for (int i = 0; i < waveIndex + 1; i++)
                returnValue += Waves[i].AdditiveSpeed;

        return (returnValue);
    }

    public Vector2 GetAdditiveGold(Vector2 value, int waveIndex)
    {
        Vector2 returnValue = value;

        if (waveIndex < Waves.Count)
            for (int i = 0; i < waveIndex + 1; i++)
                returnValue = new Vector2(returnValue.x + Waves[i].AdditiveGold, returnValue.y + Waves[i].AdditiveGold);

        return (returnValue);
    }



#if UNITY_EDITOR
    private void OnValidate()
    {
        if (renameScriptableWaves == false) return;
        foreach (ScriptableWave wave in Waves)
        {
            if (wave.name != "ScriptableWave #" + Waves.IndexOf(wave))
            {
                wave.name = "ScriptableWave #" + Waves.IndexOf(wave);
                string assetPath = AssetDatabase.GetAssetPath(wave);
                AssetDatabase.RenameAsset(assetPath, wave.name);
            }
            AssetDatabase.SaveAssetIfDirty(wave);
        }
    }
#endif
}
