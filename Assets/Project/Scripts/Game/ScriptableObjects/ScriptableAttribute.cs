using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class ScriptableAttribute : ScriptableObject
{
    [field: SerializeField] public Sprite DisplayIcon { get; private set; }
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public Color DisplayColor { get; private set; }

    public abstract bool Compare(ScriptableAttribute attribute);

    public string GetDisplayString()
    {
        if (this is ScriptableAttribute<float> floatAtr)
        {
            char thing = floatAtr.DefaultValue > 0 ? '+' : '-';
            return (DisplayName + " (" + thing + floatAtr.DefaultValue + ")");
        }
        else
            return (DisplayName);
    }
}

public abstract class ScriptableAttribute<T> : ScriptableAttribute
{
    [field: SerializeField] public ScriptableAttribute<T> Data { get; protected set; }
    [field: SerializeField] public T DefaultValue { get; protected set; }

    [SerializeField] private List<T> modifierValues = new List<T>();

    public void AddModifier(T newModifier)
    {
        modifierValues.Add(newModifier);
    }

    public T GetAttributeValue()
    {
        T returnValue = DefaultValue;

        foreach (T modifier in modifierValues)
            returnValue = AddValue(ref returnValue, modifier);

        return (returnValue);
    }

    public T GetDefaultValue()
    {
        return (DefaultValue);
    }

    protected abstract T AddValue(ref T returnValue, T modifier);

    public override bool Compare(ScriptableAttribute attribute)
    {
        return (Data == attribute);
    }
}
