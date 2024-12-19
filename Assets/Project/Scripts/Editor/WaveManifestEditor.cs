using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System;
using IterationToolkit.Editor;

[CustomEditor(typeof(WaveManifest))]
[CanEditMultipleObjects]
public class WaveManifestEditor : Editor
{
    ReorderableList reorderableList;
    ReorderableList selectedWaveList;
    WaveManifest waveManifest;
    Vector2 scrollBar;

    ScriptableWave highlightWave;

    Vector2Int waveCurveRange;

    GUIStyle primaryAltStyle;
    GUIStyle secondaryAltStyle;
    GUIStyle primaryInvalidStyle;
    GUIStyle secondaryInvalidStyle;

    private void OnEnable()
    {
        waveManifest = serializedObject.targetObject as WaveManifest;
        reorderableList = new ReorderableList(serializedObject, EditorUtilities.Seek(serializedObject, EditorUtilities.Decorate(nameof(WaveManifest.Waves))), true, true, true, true);
        reorderableList.drawElementCallback = DrawScriptableWaveList;
        reorderableList.drawHeaderCallback = DrawHeader;
        reorderableList.drawElementBackgroundCallback = DrawListingBackground;
        waveCurveRange = new Vector2Int(0, waveManifest.Waves.Count - 1);

        primaryAltStyle = EditorLabelUtilities.GetNewStyle(EditorLabelUtilities.PrimaryAlternatingColor);
        secondaryAltStyle = EditorLabelUtilities.GetNewStyle(EditorLabelUtilities.SecondaryAlternatingColor);
        primaryInvalidStyle = EditorLabelUtilities.GetNewStyle(new Color(0.1f,0.05f,0.05f,1f));
        secondaryInvalidStyle = EditorLabelUtilities.GetNewStyle(new Color(0.75f, 0.05f, 0.05f, 1f));
    }

    public override void OnInspectorGUI()
    {
        highlightWave = null;
        serializedObject.Update();
        if (reorderableList != null)
        {
            scrollBar = EditorGUILayout.BeginScrollView(scrollBar, GUILayout.MaxHeight(350));
            reorderableList.DoLayoutList();
            EditorGUILayout.EndScrollView();
            DrawWaveStatistics();
        }

        //EditorUtilities.DrawWaveCurves(waveManifest, ref waveCurveRange);


        if (highlightWave != null)
        {
            bool useLessBool = false;
            int index = waveManifest.Waves.IndexOf(highlightWave);
            EditorUtilities.DrawSpawnInfoStatistics(highlightWave, ref useLessBool, ref index, false);
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawHeader(Rect rect) => EditorGUI.LabelField(rect, "Waves", EditorStyles.boldLabel);

    private void DrawListingBackground(Rect listRest, int index, bool isActive, bool isFocused)
    {
        GUIStyle style = EditorLabelUtilities.GetAlternatingStyle(primaryAltStyle, secondaryAltStyle, index);
        GUI.DrawTexture(listRest, style.normal.background);
    }

    private void DrawScriptableWaveList(Rect listRect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty waveProp = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
        ScriptableWave wave = waveManifest.Waves[index];
        DynamicRect rect = new DynamicRect(listRect, 1f, 0f, 5f, 0f, 0f, EditorGUIUtility.singleLineHeight);
        //EditorGUI.SelectableLabel(dynamicRect.Get(100), "Spawn Info #" + index + ":");
        rect.Modify(y: EditorGUIUtility.standardVerticalSpacing);
        EditorUtilities.DrawField(waveProp, rect, "Wave #" + index, 100, 300, false);
        EditorUtilities.DrawField(wave.GetTotalTime() + "s", rect, "Length", 50, 35, true);
        EditorUtilities.DrawField(wave.GetTotalEnemyCount(), rect, "Enemies", 55, 35, true);
        EditorUtilities.DrawField(wave.GetTotalEnemyHealth(index), rect, "Health", 45, 35, true);
        EditorUtilities.DrawField(wave.GetTotalEnemyGold(index).x, rect, "Gold", 30, 35, true);
        EditorUtilities.DrawField(wave.GetTotalEnemyGold(index).y, rect, string.Empty, 55, 35, true);

        if (isActive)
        {
            highlightWave = wave;
        }
    }

    private void DrawWaveStatistics()
    {

    }
}
