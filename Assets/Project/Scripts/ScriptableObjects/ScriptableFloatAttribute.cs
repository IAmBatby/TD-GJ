using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableAttribute", menuName = "TD-GJ/ScriptableFloatAttribute", order = 1)]
public class ScriptableFloatAttribute : ScriptableAttribute<float>
{

    public static ScriptableFloatAttribute Create(ScriptableFloatAttribute referenceData, float newDefaultValue)
    {
        Debug.Log("Instansiating Live: " + referenceData.name);
        ScriptableFloatAttribute instansiatedAttribute = ScriptableObject.Instantiate(referenceData);
        instansiatedAttribute.DefaultValue = newDefaultValue;
        instansiatedAttribute.Data = referenceData;
        instansiatedAttribute.name = "Instansiated " + instansiatedAttribute.name;
        return (instansiatedAttribute);
    }

    protected override float AddValue(ref float returnValue, float modifier) => returnValue + modifier;
}
