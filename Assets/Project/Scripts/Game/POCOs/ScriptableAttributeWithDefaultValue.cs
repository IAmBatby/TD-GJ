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
        ScriptableFloatAttribute newAttribute = ScriptableFloatAttribute.Create(reference.Attribute, reference.Value);
        Value = reference.Value;
        Attribute = newAttribute;
    }

    public void ApplyWithNewReference(ScriptableAttribute atr, float newValue)
    {
        if (atr is ScriptableFloatAttribute floatAtr)
        {
            ScriptableFloatAttribute newAttribute = ScriptableFloatAttribute.Create(floatAtr, newValue);
            Value = newValue;
            Attribute = newAttribute;
        }
    }
}

