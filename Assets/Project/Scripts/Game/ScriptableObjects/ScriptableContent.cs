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
    [field: SerializeField] public string ContentName { get; protected set; } = string.Empty;
    [field: SerializeField] public Sprite ContentIcon { get; protected set; }
    [field: SerializeField] public Color ContentColor { get; protected set; } = Color.black;
    [field: SerializeField] public Texture2D Cursor { get; private set; }
    [field: SerializeField] public bool Highlightable { get; private set; } = true;
    [field: SerializeField] public ReactionInfo OnSpawnReaction { get; private set; }

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

    public static void SpawnPrefab(ScriptableContent content)
    {
        content.SpawnPrefab().transform.position = GameManager.Player.transform.position + GameManager.Player.transform.forward;
    }


    protected virtual ContentBehaviour SpawnPrefabSetup(ContentBehaviour newBehaviour)
    {
        return (newBehaviour);
    }

    public virtual string GetCategoryName() => "Content";
    public virtual string GetDisplayName() => ContentName;
    public virtual Sprite GetDisplayIcon() => ContentIcon;
    public virtual Color GetDisplayColor() => ContentColor;
}
