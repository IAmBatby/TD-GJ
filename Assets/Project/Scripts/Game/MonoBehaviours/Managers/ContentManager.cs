using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContentManager : GlobalManager
{
    public static new ContentManager Instance => SingletonManager.GetSingleton<ContentManager>(typeof(ContentManager));

    [SerializeField] private List<ContentBehaviour> fullBehaviourList = new List<ContentBehaviour>();
    [SerializeField] private List<ContentCollection> contentCollections = new List<ContentCollection>();

    public static void RegisterBehaviour<T>(T behaviour) where T : ContentBehaviour
    {
        Instance.fullBehaviourList.Add(behaviour);
        bool foundCollection = false;
        foreach (ContentCollection contentCollection in Instance.contentCollections)
            if (contentCollection is ContentCollection<T> castedCollection)
            {
                castedCollection.RegisterBehaviour(behaviour);
                foundCollection = true;
            }

        if (foundCollection == false)
        {
            ContentCollection<T> newCollection = new ContentCollection<T>();
            newCollection.RegisterBehaviour(behaviour);
            Instance.contentCollections.Add(newCollection);
        }
    }

    public static void UnregisterBehaviour<T>(T behaviour, bool destroyOnUnregistration) where T: ContentBehaviour
    {
        if (behaviour == null)
        {
            Debug.LogWarning("Tried to unregister null behaviour!");
            return;
        }
        Instance.fullBehaviourList.Remove(behaviour);
        foreach (ContentCollection contentCollection in Instance.contentCollections)
            if (contentCollection is ContentCollection<T> castedCollection)
                castedCollection.UnregisterBehaviour(behaviour);

        if (destroyOnUnregistration == true)
        {
            behaviour.enabled = false;
            behaviour.gameObject.SetActive(false);
            GameObject.Destroy(behaviour.gameObject);
        }
    }

    public static List<T> GetBehaviours<T>() where T : ContentBehaviour
    {
        foreach (ContentCollection collection in Instance.contentCollections)
            if (collection is ContentCollection<T> castedCollection)
                return (castedCollection.GetBehaviours());
        return (new List<T>());
    }

    public static List<T> GetBehaviours<T>(ScriptableContent filter) where T : ContentBehaviour
    {
        foreach (ContentCollection collection in Instance.contentCollections)
            if (collection is ContentCollection<T> castedCollection)
                return (castedCollection.GetBehaviours(filter));
        return (new List<T>());
    }

    public static void DebugAllContent()
    {
        DebugContent<ContentBehaviour>();
        DebugContent<ItemBehaviour>();
        DebugContent<TurretBaseBehaviour>();
        DebugContent<TurretModuleBehaviour>();
        DebugContent<UpgradeBehaviour>();
        DebugContent<ProjectileBehaviour>();
        DebugContent<HurtableBehaviour>();
        DebugContent<PlayerBehaviour>();
        DebugContent<EnemyBehaviour>();
    }

    public static void DebugContent<T>() where T : ContentBehaviour
    {
        List<T> registeredContents = GetBehaviours<T>();
        List<T> scrapedContents = new List<T>(Object.FindObjectsOfType<T>());

        ContentCollection<T> collection = null;
        foreach (ContentCollection existingCollection in Instance.contentCollections)
            if (existingCollection is ContentCollection<T> castedCollection)
                collection = castedCollection;

        if (collection == null)
        {
            Debug.LogWarning("Failed To Debug: " + typeof(T).Name + " (This could be because no content of this type exists yet!");
            return;
        }

        bool validated = collection.RegistrationCount - scrapedContents.Count == collection.ActiveCount;

        string debugLog = string.Empty;
        debugLog += "ContentManager Report: " + typeof(T).Name;
        debugLog += validated == true ? " (TL;DR: Looks Good!)" + "\n" : " (TL;DR: Looks Bad!)" + "\n";
        debugLog += "Registered Contents List Count: #" + registeredContents.Count + ", Scraped Contents List Count: #" + scrapedContents.Count + "\n";
        debugLog += "Active Count: #" + collection.ActiveCount + " , Registration Count: #" + collection.RegistrationCount + ", Unregistration Count: #" + collection.UnregistrationCount + "\n";
        debugLog += "List Count Comparison: (" + registeredContents.Count + " / " + scrapedContents.Count + "), Registration Count Comparison: (" + collection.RegistrationCount + " / " + scrapedContents.Count + ")";

        if (validated == true)
            Debug.Log(debugLog);
        else
            Debug.LogError(debugLog);
    }
}
