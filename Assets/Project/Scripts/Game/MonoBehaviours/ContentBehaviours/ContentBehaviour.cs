using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentBehaviour : MonoBehaviour, IHighlightable
{
    public ScriptableContent ContentData { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    //public AudioPlayer AudioPlayer { get; private set; }
    //public ParticlePlayer ParticlePlayer { get; private set; }
    public ReactionPlayer ReactionPlayer { get; private set; }
    public List<Renderer> Renderers { get; private set; } = new List<Renderer>();
    public List<Collider> Colliders { get; private set; } = new List<Collider>();
    private Dictionary<Collider, bool> initialColliderActivityStates = new Dictionary<Collider, bool>();

    protected MaterialController MaterialController { get; private set; }

    public ExtendedEvent<bool> OnMouseoverToggle = new ExtendedEvent<bool>();

    public ExtendedEvent OnDespawn { get; private set; } = new ExtendedEvent();

    private List<ContentDisplayListing> contentDisplayListings = new List<ContentDisplayListing>();

    public bool HighlightingDisabled { get; set; }

    public bool IsHighlighted { get; set; }

    public bool IsSetToBeKilled { get; set; }   

    public ContentDisplayInfo GeneralDisplayInfo { get; private set; }
    public ContentDisplayListing GeneralDisplayListing { get; private set; }

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

    private void OnMouseOver()
    {
        GameManager.Instance.OnContentBehaviourMousedOver(this);
    }

    public void SetData(ScriptableContent content) => ContentData = content;

    public void Initialize()
    {
        ReactionPlayer = ReactionPlayer.Create(this);
        ReactionPlayer.Play(ContentData.OnSpawnReaction);

        Rigidbody = GetComponent<Rigidbody>();
        if (Rigidbody == null)
            Debug.LogError("Failed To Find Rigidbody!", transform);

        foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
            Renderers.Add(renderer);
        foreach (Collider collider in gameObject.GetComponentsInChildren<Collider>())
        {
            Colliders.Add(collider);
            initialColliderActivityStates.Add(collider, collider.enabled);
        }
        MaterialController = new MaterialController(Renderers);

        GeneralDisplayInfo = GetGeneralDisplayInfo();
        GeneralDisplayListing = new ContentDisplayListing(GeneralDisplayInfo);
        contentDisplayListings.Add(GeneralDisplayListing);
        OnSpawn();

        GameManager.OnHighlightChanged.AddListener(HighlightObject);
    }

    protected virtual void OnSpawn() { }

    public void AddContentDisplayListing(ContentDisplayListing contentDisplayListing)
    {
        if (!contentDisplayListings.Contains(contentDisplayListing))
            contentDisplayListings.Add(contentDisplayListing);
    }

    public void RemoveContentDisplayListing(ContentDisplayListing contentDisplayListing)
    {
        if (contentDisplayListings.Contains(contentDisplayListing))
            contentDisplayListings.Remove(contentDisplayListing);
    }

    private void HighlightObject(IHighlightable hightable)
    {
        if (hightable != null && hightable.Compare(gameObject))
            IsHighlighted = true;
        else
            IsHighlighted = false;
    }

    public void OverrideCollisions(bool enableOverride, bool overrideValue = false, bool onlyTriggers = false)
    {
        if (enableOverride == true)
        {
            foreach (Collider collider in Colliders)
                if (onlyTriggers == false || (onlyTriggers == true && collider.isTrigger))
                    collider.enabled = overrideValue;
        }
        else
        {
            foreach (KeyValuePair<Collider, bool> kvp in initialColliderActivityStates)
                kvp.Key.enabled = kvp.Value;
        }

    }

    protected virtual ContentDisplayInfo GetGeneralDisplayInfo() => ContentData.CreateGeneralDisplayInfo();


    public List<ContentDisplayListing> GetDisplayListings() => new List<ContentDisplayListing>(contentDisplayListings);

    public Texture2D GetCursor() => ContentData.Cursor;
    public bool IsHighlightable() => HighlightingDisabled ? false : ContentData.Highlightable;
    public bool Compare(GameObject go) => (go != null && go == gameObject);
    public virtual List<Renderer> GetRenderers() => Renderers;
    public Color GetColor() => ContentData.GetDisplayColor();
    public MaterialController GetMaterialController() => MaterialController;

    public virtual void RegisterBehaviour()
    {
        CheatManager.RegisterCheat(ScriptableContent.SpawnPrefab, ContentData, "Spawning: " + ContentData.GetCategoryName());
        ContentManager.RegisterBehaviour(this);
    }
    public virtual void UnregisterBehaviour(bool destroyOnUnregistration)
    {
        OnDespawn.Invoke();
        GameManager.OnHighlightChanged.RemoveListener(HighlightObject);
        ContentManager.UnregisterBehaviour(this, destroyOnUnregistration);
    }
}
