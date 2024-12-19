using IterationToolkit;
using IterationToolkit.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/*
[CustomPropertyDrawer(typeof(SpawnInfo))]
public class SpawnInfoProperty : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty timeToSpawnProp = Seek(property, Decorate(nameof(SpawnInfo.TimeToSpawn)));
        SerializedProperty enemyToSpawnProp = Seek(property, Decorate(nameof(SpawnInfo.EnemyToSpawn)));
        SerializedProperty amountToSpawnProp = Seek(property, Decorate(nameof(SpawnInfo.AmountToSpawn)));
        EditorGUI.BeginProperty(position, label, property);
        Rect firstRect = new Rect(position.x, position.y, position.width / 2.2f, position.height);
        Rect secondRect = new Rect(position.x + (position.width / 2.2f) + 15, position.y, (position.width / 3.5f) - 15, position.height);
        Rect thirdRect = new Rect(position.x + ((position.width / 2.2f)) + ((position.width / 3.5f) + 15), position.y, position.width / 6f, position.height);  
        EditorGUI.PropertyField(firstRect, timeToSpawnProp, label);
        EditorGUI.PropertyField(secondRect, enemyToSpawnProp, GUIContent.none);
        EditorGUI.PropertyField(thirdRect, amountToSpawnProp, GUIContent.none);
        EditorGUI.EndProperty();
        property.serializedObject.ApplyModifiedProperties();
    }



    private string Decorate(string name) => "<" + name + ">k__BackingField";

    private SerializedProperty Seek(SerializedProperty target, string targetField)
    {
        foreach (SerializedProperty sp in EditorLabelUtilities.FindSerializedProperties(target.Copy()))
            if (sp.name.Contains(targetField) || sp.name.Contains(Decorate(targetField)))
                return (sp.Copy());
        return (null);
    }
}
*/
