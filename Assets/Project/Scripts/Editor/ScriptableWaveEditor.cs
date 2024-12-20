using IterationToolkit.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(ScriptableWave))]
[CanEditMultipleObjects]
public class SpawnInfoListProperty : Editor
{
    ReorderableList reorderableList;
    ScriptableWave wave;
    int simulatedWaveIndex = 0;
    Rect previousSize;
    private bool useScaling;
    private SerializedProperty intermissionTimeLengthProp;
    private SerializedProperty additiveHealthProp;
    private SerializedProperty additiveSpeedProp;
    private SerializedProperty additiveGoldProp;

    private void OnEnable()
    {
        previousSize = new Rect(0, 0, 0, 0);
        wave = serializedObject.targetObject as ScriptableWave;
        intermissionTimeLengthProp = EditorUtilities.Seek(serializedObject.GetIterator(), nameof(ScriptableWave.IntermissionTimeLength));
        reorderableList = new ReorderableList(serializedObject, EditorUtilities.Seek(serializedObject.GetIterator(), nameof(ScriptableWave.WaveSpawnInfos)), true, true, true, true);
        reorderableList.drawElementCallback = DrawSpawnInfoColumn;
        reorderableList.drawHeaderCallback = DrawHeader;

        additiveGoldProp = EditorUtilities.Seek(serializedObject, nameof(ScriptableWave.AdditiveGold));
        additiveHealthProp = EditorUtilities.Seek(serializedObject, nameof(ScriptableWave.AdditiveHealth));
        additiveSpeedProp = EditorUtilities.Seek(serializedObject, nameof(ScriptableWave.AdditiveSpeed));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(intermissionTimeLengthProp);
        EditorGUILayout.Space(10);
        EditorGUILayout.EndVertical();
        reorderableList.DoLayoutList();
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Additive Health: ", GUILayout.Width(90));
        EditorGUILayout.PropertyField(additiveHealthProp, GUIContent.none, GUILayout.Width(30));
        EditorGUILayout.Space(5, false);
        EditorGUILayout.LabelField("Additive Gold: ", GUILayout.Width(85));
        EditorGUILayout.PropertyField(additiveGoldProp, GUIContent.none, GUILayout.Width(30));
        EditorGUILayout.Space(5, false);
        EditorGUILayout.LabelField("Additive Speed: ", GUILayout.Width(90));
        EditorGUILayout.PropertyField(additiveSpeedProp, GUIContent.none, GUILayout.Width(30));

        EditorGUILayout.EndHorizontal();
        EditorUtilities.DrawSpawnInfoStatistics(wave, ref useScaling, ref simulatedWaveIndex, true);
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawHeader(Rect rect) => EditorGUI.LabelField(rect, "Spawn Infos List", EditorStyles.boldLabel);

    private void DrawSpawnInfoColumn(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty spawnInfo = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
        EditorUtilities.DrawSpawnInfoListing(spawnInfo, rect, index, isActive, isFocused);
    }
}
