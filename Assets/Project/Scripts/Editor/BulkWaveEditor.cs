using IterationToolkit.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BulkWaveEditor : BulkScriptableObjectEditorWindow<ScriptableWave>
{
    [MenuItem("Tools/Bulk Wave Editor")]
    public static void OpenWindow() => GetWindow<BulkWaveEditor>().InitializeWindow();

    //protected override Type[] GetParentObjects(ScriptableWave childObject) => new[] { childObject.GetType() };
}
