using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentSpawner : MonoBehaviour
{
    public enum SpawnMode { None, OnEnable, Awake, Start}
    [field: SerializeField] public ScriptableContent DefaultContent { get; private set; }
    [field: SerializeField] private SpawnMode DefaultSpawnMode = SpawnMode.Awake;
    [SerializeField] private LayerMask dropMask;

    private void OnEnable()
    {
        ExtendedEvent defaultSpawnEvent = DefaultSpawnMode switch
        {
            SpawnMode.OnEnable => GameManager.OnGameManagerEnable,
            SpawnMode.Awake => GameManager.OnGameManagerAwake,
            SpawnMode.Start => GameManager.OnGameManagerStart,
            SpawnMode.None => null,
            _ => null,
        };

        if (defaultSpawnEvent == null ) return;

        defaultSpawnEvent.AddListener(Spawn);
    }

    private void Spawn() => Spawn(DefaultContent);

    public void SetContent(ScriptableContent newDefault) => DefaultContent = newDefault;

    public ContentBehaviour Spawn(ScriptableContent content = null)
    {
        if (content == null)
            content = DefaultContent;

        if (content == null)
        {
            Debug.LogError("ContentSpawner Has Null Content", transform);
            return (null);
        }

        ContentBehaviour newContent = content.SpawnPrefab(null);
        if (content.ShouldBeAnchoredToGround && Physics.Raycast(transform.position, new Vector3(transform.position.x, -5000, transform.position.z), out RaycastHit hit, Mathf.Infinity, dropMask))
                newContent.transform.position = hit.point;
        else
            newContent.transform.position = transform.position;

        return (newContent);
    }

    private List<MeshFilter> allRenderers = new List<MeshFilter>();
    private void OnDrawGizmos()
    {
        if (DefaultContent == null || DefaultContent.Prefab == null) return;
        Utilities.DrawPrefabPreview(transform, DefaultContent.Prefab.gameObject, DefaultContent.GetDisplayColor(), Color.yellow, ref allRenderers);
    }
}
