using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScriptableFloatAttributeWithDefaultValue
{
    [field: SerializeField] public ScriptableFloatAttribute Attribute { get; private set; }
    [field: SerializeField] public float Value { get; private set; }

    public void Instansiate()
    {
        Debug.Log("Instansiating : " + Attribute.name);
        ScriptableFloatAttribute newAttribute = (ScriptableFloatAttribute)ScriptableFloatAttribute.Create(Attribute, Value);
        Debug.Log("New Attribute Is: " + newAttribute.name);
        Attribute = newAttribute;
    }

    public void ApplyWithReference(ScriptableFloatAttributeWithDefaultValue reference)
    {
        ScriptableFloatAttribute newAttribute = (ScriptableFloatAttribute)ScriptableFloatAttribute.Create(reference.Attribute, reference.Value);
        Attribute = newAttribute;
    }
}
[Serializable]
public class ScriptableIntAttributeWithDefaultValue
{
    [field: SerializeField] public ScriptableIntAttribute Attribute { get; private set; }
    [field: SerializeField] public int Value { get; private set; }

    public void Instansiate()
    {
        Debug.Log("Instansiating : " + Attribute.name);
        ScriptableIntAttribute newAttribute = (ScriptableIntAttribute)ScriptableIntAttribute.Create(Attribute, Value);
        Debug.Log("New Attribute Is: " + newAttribute.name);
        Attribute = newAttribute;
    }

    public void ApplyWithReference(ScriptableIntAttributeWithDefaultValue reference)
    {
        ScriptableIntAttribute newAttribute = (ScriptableIntAttribute)ScriptableIntAttribute.Create(reference.Attribute, reference.Value);
        Attribute = newAttribute;
    }
}
