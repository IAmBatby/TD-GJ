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
        if (DefaultSpawnMode == SpawnMode.OnEnable)
            Spawn(DefaultContent);
    }

    private void Awake()
    {
        if (DefaultSpawnMode == SpawnMode.Awake)
            Spawn(DefaultContent);
    }

    private void Start()
    {
        if (DefaultSpawnMode == SpawnMode.Start)
            Spawn(DefaultContent);
    }

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
}
