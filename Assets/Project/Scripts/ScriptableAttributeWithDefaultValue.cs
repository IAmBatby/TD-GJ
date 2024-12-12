using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScriptableAttributeWithDefaultValue<T>
{
    [field: SerializeField] public ScriptableAttribute<T> Attribute { get; private set; }
    [field: SerializeField] public T Value { get; private set; }

    public void Instansiate()
    {
        Debug.Log("Instansiating : " + Attribute.name);
        ScriptableAttribute<T> newAttribute = ScriptableAttribute<T>.Create(Attribute, Value);
        Debug.Log("New Attribute Is: " + newAttribute.name);
        Attribute = newAttribute;
    }

    public void ApplyWithReference(ScriptableAttributeWithDefaultValue<T> reference)
    {
        ScriptableAttribute<T> newAttribute = ScriptableAttribute<T>.Create(reference.Attribute, reference.Value);
        Attribute = newAttribute;
    }
}
[Serializable]
public class ScriptableFloatAttributeWithDefaultValue : ScriptableAttributeWithDefaultValue<float>
{

}
[Serializable]
public class ScriptableIntAttributeWithDefaultValue : ScriptableAttributeWithDefaultValue<int>
{

}
