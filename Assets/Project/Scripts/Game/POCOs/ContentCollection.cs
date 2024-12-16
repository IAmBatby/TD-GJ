using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ContentCollection
{

}

[System.Serializable]
public class ContentCollection<T> : ContentCollection where T : ContentBehaviour
{
    [SerializeField] private List<T> registeredContents = new List<T>();
    private Dictionary<ScriptableContent, List<T>> registeredContentsDict = new Dictionary<ScriptableContent, List<T>>();

    public int RegistrationCount { get; private set; }
    public int UnregistrationCount { get; private set; }
    public int ActiveCount => RegistrationCount - UnregistrationCount;

    public void RegisterBehaviour(T behaviour)
    {
        if (!registeredContents.Contains(behaviour))
            registeredContents.Add(behaviour);

        if (registeredContentsDict.TryGetValue(behaviour.ContentData, out List<T> behaviours))
            behaviours.Add(behaviour);
        else
            registeredContentsDict.Add(behaviour.ContentData, new List<T>() { behaviour });

        RegistrationCount++;
    }

    public void UnregisterBehaviour(T behaviour)
    {
        if (registeredContents.Contains(behaviour))
            registeredContents.Remove(behaviour);

        if (registeredContentsDict.TryGetValue(behaviour.ContentData, out List<T> behaviours))
        {
            if (behaviours.Count > 1)
                behaviours.Remove(behaviour);
            else
                registeredContentsDict.Remove(behaviour.ContentData);
        }

        UnregistrationCount++;
    }

    public List<T> GetBehaviours() => registeredContents;

    public List<T> GetBehaviours(ScriptableContent filter)
    {
        if (registeredContentsDict.TryGetValue(filter, out List<T> behaviours))
            return (behaviours);
        else
            return (new List<T>());
    }
}
