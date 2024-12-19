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

    private void OnEnable()
    {
        previousSize = new Rect(0, 0, 0, 0);
        wave = serializedObject.targetObject as ScriptableWave;
        intermissionTimeLengthProp = EditorUtilities.Seek(serializedObject.GetIterator(), nameof(ScriptableWave.IntermissionTimeLength));
        reorderableList = new ReorderableList(serializedObject, EditorUtilities.Seek(serializedObject.GetIterator(), nameof(ScriptableWave.WaveSpawnInfos)), true, true, true, true);
        reorderableList.drawElementCallback = DrawSpawnInfoColumn;
        reorderableList.drawHeaderCallback = DrawHeader;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(intermissionTimeLengthProp);
        EditorGUILayout.Space(10);
        EditorGUILayout.EndVertical();
        reorderableList.DoLayoutList();
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
