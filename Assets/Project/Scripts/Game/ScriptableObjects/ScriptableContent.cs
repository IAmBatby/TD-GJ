using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableContent", menuName = "TD-GJ/ScriptableContents/ScriptableContent", order = 1)]
public class ScriptableContent : ScriptableObject
{
    [field: Header("Required References")]
    [field: SerializeField] public ContentBehaviour Prefab { get; private set; }

    [field: Header("Optional References"), Space(15)]
    [field: SerializeField] public string DisplayName { get; private set; } = string.Empty;
    [field: SerializeField] public Texture2D Cursor { get; private set; }
    [field: SerializeField] public bool Highlightable { get; private set; } = true;
    [field: SerializeField] public AudioPreset OnSpawnAudio { get; private set; }

    [field: Header("Content Values"), Space(15)]
    [field: SerializeField] public bool ShouldBeAnchoredToGround { get; private set; }

    public ContentBehaviour SpawnPrefab(Transform parent = null)
    {
        ContentBehaviour spawnedBehaviour = GameObject.Instantiate(Prefab, parent);
        spawnedBehaviour.SetData(this);
        GameManager.RegisterNewContentBehaviour(spawnedBehaviour);
        spawnedBehaviour.Initialize();
        return (SpawnPrefabSetup(spawnedBehaviour));
    }

    protected virtual ContentBehaviour SpawnPrefabSetup(ContentBehaviour newBehaviour)
    {
        return (newBehaviour);
    }
}
