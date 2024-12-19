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
