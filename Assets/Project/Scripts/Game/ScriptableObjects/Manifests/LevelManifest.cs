using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelManifest", menuName = "TD-GJ/Manifests/LevelManifest", order = 1)]
public class LevelManifest : ScriptableObject
{
    [field: SerializeField] public List<ScriptableLevel> allLevels = new List<ScriptableLevel>();


    public Dictionary<string, ScriptableLevel> GetLevelSceneDict()
    {
        Dictionary<string, ScriptableLevel> levels = new Dictionary<string, ScriptableLevel>();

        foreach (ScriptableLevel level in allLevels)
            if (!levels.ContainsKey(level.SceneName))
                levels.Add(level.SceneName, level);

        return (levels);
    }
}
