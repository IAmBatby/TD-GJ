using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentBehaviour : MonoBehaviour, IHighlightable
{
    public ScriptableContent ContentData { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    public AudioPlayer AudioPlayer { get; private set; }

    public ExtendedEvent<bool> OnMouseoverToggle = new ExtendedEvent<bool>();

    private List<ContentDisplayInfo> contentDisplayInfos = new List<ContentDisplayInfo>();

    public bool IsHighlighted => GameManager.Instance.HighlightedObject != null && GameManager.Instance.HighlightedObject.Compare(gameObject);

    public ContentDisplayInfo GeneralDisplayInfo { get; private set; }

    private void OnMouseEnter()
    {
        GameManager.Instance.OnContentBehaviourMousedEnter(this);
        OnMouseoverToggle.Invoke(true);
    }

    private void OnMouseExit()
    {
        GameManager.Instance.OnContentBehaviourMousedExit(this);
        OnMouseoverToggle.Invoke(false);
    }

    public void SetData(ScriptableContent content) => ContentData = content;

    public void Initialize()
    {
        AudioPlayer = AudioPlayer.Create(this);
        AudioPlayer.PlayAudio(ContentData.OnSpawnAudio);

        Rigidbody = GetComponent<Rigidbody>();
        if (Rigidbody == null)
            Debug.LogError("Failed To Find Rigidbody!", transform);

        GeneralDisplayInfo = CreateGeneralDisplayInfo();
        contentDisplayInfos.Add(GeneralDisplayInfo);
        OnSpawn();
    }

    protected virtual void OnSpawn() { }

    public void AddContentDisplayInfo(ContentDisplayInfo contentDisplayInfo)
    {
        if (!contentDisplayInfos.Contains(contentDisplayInfo))
            contentDisplayInfos.Add(contentDisplayInfo);
    }

    public void RemoveContentDisplayInfo(ContentDisplayInfo contentDisplayInfo)
    {
        if (contentDisplayInfos.Contains(contentDisplayInfo))
            contentDisplayInfos.Remove(contentDisplayInfo);
    }

    protected virtual ContentDisplayInfo CreateGeneralDisplayInfo() => new ContentDisplayInfo(ContentData.ContentName, displayIcon: ContentData.ContentIcon, displayColor: ContentData.ContentColor);


    public List<ContentDisplayInfo> GetDisplayInfos() => new List<ContentDisplayInfo>(contentDisplayInfos);
    public Texture2D GetCursor() => ContentData.Cursor;
    public bool IsHighlightable() => ContentData.Highlightable;
    public bool Compare(GameObject go) => (go == gameObject);

    public virtual void RegisterBehaviour() => ContentManager.RegisterBehaviour(this);
    public virtual void UnregisterBehaviour(bool destroyOnUnregistration) => ContentManager.UnregisterBehaviour(this, destroyOnUnregistration);
}
