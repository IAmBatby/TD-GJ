using IterationToolkit.Editor;
using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

public class BulkContentEditor : BulkScriptableObjectEditorWindow<ScriptableContent, Type>
{
    [MenuItem("Tools/Bulk Item Editor")]
    public static void OpenWindow() => GetWindow<BulkContentEditor>().InitializeWindow();

    protected override Type[] GetParentObjects(ScriptableContent childObject) => new[] { childObject.GetType() };
}
