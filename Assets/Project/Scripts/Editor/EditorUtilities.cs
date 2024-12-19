using IterationToolkit.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public static class EditorUtilities
{
    public static void DrawSpawnInfoStatistics(ScriptableWave wave, ref bool useScaling, ref int waveIndex, bool allowIndexChange)
    {
        int waveIndexThatGetsUsed = 0;
        int enemyCount = wave.GetTotalEnemyCount();
        float totalTime = wave.GetTotalTime();
        float finalTime = wave.GetFinalTime();
        Dictionary<ScriptableEnemy, int> enemies = wave.GetEnemyCountDict();

        GUIStyle background = EditorLabelUtilities.GetNewStyle();
        EditorGUILayout.Space(25);
        Rect spaceRect = EditorGUILayout.BeginVertical(background);
        spaceRect = GUILayoutUtility.GetRect(spaceRect.x, spaceRect.y, 500, 1200);
        DynamicRect rect = new DynamicRect(spaceRect, 1f, 0f, 10f, 0f, 0f, EditorGUIUtility.singleLineHeight);
        GUIStyle style = new GUIStyle(EditorStyles.whiteLargeLabel);
        style.normal.background = background.normal.background;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = EditorStyles.boldLabel.normal.textColor;
        EditorGUI.SelectableLabel(rect.Get(200), "Generated Wave Statistics", style);
        float revertX = rect.Additive.x;
        rect.Modify(addX: 55);
        if (allowIndexChange)
            useScaling = DrawField(useScaling, rect, "Use Scaling", 100, EditorGUIUtility.singleLineHeight, false);
        if (useScaling && allowIndexChange)
        {
            waveIndex = DrawField(waveIndex, rect, "Simulated Index", 100, 30, false);
            waveIndexThatGetsUsed = waveIndex;
        }
        else if (allowIndexChange == false)
        {
            waveIndex = DrawField(waveIndex, rect, "Simulated Index", 100, 30, true);
            waveIndexThatGetsUsed = 0;
        }
        rect.Set(addX: revertX);
        rect.Modify(x: 15, y: 35, addX: -200);
        revertX = rect.Additive.x;

        float totalHealth = wave.GetTotalEnemyHealth(waveIndexThatGetsUsed);
        Vector2 totalGold = wave.GetTotalEnemyGold(waveIndexThatGetsUsed);

        EditorGUI.SelectableLabel(rect.Get(140), "Timings", EditorStyles.boldLabel);
        rect.Modify(y: 20, addX: -140);
        DrawField(finalTime + "s", rect, "Last Spawn Period", 110, 40, true);
        rect.Modify(addX: 35f);
        DrawField(totalTime + "s", rect, "Active Wave Length", 115, 40, true);
        //rect.Modify(addX: 10f);
        DrawField((totalTime + wave.IntermissionTimeLength) + "s", rect, "Full Wave Length", 110, 40, true);
        rect.Set(addX: revertX);
        rect.Modify(y: 25);
        revertX = rect.Additive.x;
        EditorGUI.SelectableLabel(rect.Get(140), "Enemies", EditorStyles.boldLabel);

        rect.Modify(y: 20, addX: -140);
        if (enemyCount > 0 && finalTime != 0)
            DrawField((enemyCount / finalTime).ToString("F2") + "s", rect, "Avg. Spawn Per Second", 185f, 40, true);
        else
            DrawField("N / A", rect, "Avg. Spawn Per Second", 185f, 40, true);
        DrawField(enemyCount, rect, "Total Amount", 80, 35, true);
        DrawField(totalHealth, rect, "Total Health", 100, 50, true);
        DrawField(totalGold.x, rect, "Total Gold", 100, 50, true);
        DrawField(totalGold.y, rect, string.Empty, 0, 50, true);
        rect.Set(addX: revertX);
        rect.Modify(x: 10, y: 25);

        foreach (KeyValuePair<ScriptableEnemy, int> kvp in enemies)
        {
            revertX = rect.Additive.x;
            DrawField<ScriptableEnemy>(objectValue: kvp.Key, rect, "Enemy", 50, 205, true);
            DrawField(kvp.Value, rect, "Amount", 50, 35, true);
            DrawField(wave.GetCombinedEnemyHealth(kvp.Key, waveIndexThatGetsUsed), rect, "Combined Health", 100, 50, true);
            DrawField(wave.GetCombinedEnemyGold(kvp.Key, waveIndexThatGetsUsed).x, rect, "Combined Gold", 100, 50, true);
            DrawField(wave.GetCombinedEnemyGold(kvp.Key, waveIndexThatGetsUsed).y, rect, string.Empty, 0, 50, true);
            rect.Set(addX: revertX);
            rect.Modify(y: 25);
        }
        EditorGUILayout.EndVertical();
    }

    public static void DrawWaveCurves(WaveManifest manifest, ref Vector2Int range)
    {
        AnimationCurve enemyCount = new AnimationCurve();
        AnimationCurve goldCount = new AnimationCurve();
        AnimationCurve healthCount = new AnimationCurve();
        List<SavedWaveInfo> waveInfos = new List<SavedWaveInfo>();
        for (int i = 0; i < manifest.Waves.Count; i++)
            waveInfos.Add(new SavedWaveInfo(manifest.Waves[i], i));

        Rect curveRect = GUILayoutUtility.GetRect(800, 350);
        curveRect.y += 10;
        range = EditorGUI.Vector2IntField(curveRect, "Wave Range", range);
        curveRect.y += 25;
        if (range.x < 0) range.x = 0;
        if (range.y > waveInfos.Count - 1) range.y = waveInfos.Count - 1;

        for (int i = 0; i < waveInfos.Count; i++)
            if (/*waveInfos[i].IsValid == true &&*/ i >= range.x && i <= range.y)
            {
                enemyCount.AddKey(waveInfos[i].EnemyCount * i, waveInfos[i].EnemyCount);
                goldCount.AddKey(Mathf.Lerp(waveInfos[i].TotalGold.x, waveInfos[i].TotalGold.y, 0.5f) * i, Mathf.Lerp(waveInfos[i].TotalGold.x, waveInfos[i].TotalGold.y, 0.5f));
                healthCount.AddKey(waveInfos[i].TotalHealth * i, waveInfos[i].TotalHealth);
            }

        GUIStyle style = EditorStyles.colorField;

        Color temp = GUI.backgroundColor;
        Rect rect = EditorGUILayout.BeginVertical(style);
        EditorGUILayout.EndVertical();
        //Rect curveRect = GUILayoutUtility.GetRect(rect.x, rect.y + 50, rect.width, rect.width / 3);
        curveRect.height -= 50f;
        Color backgroundColor = EditorLabelUtilities.SecondaryAlternatingColor;
        Color overlayColor = new Color(0, 0, 0, 0);
        EditorGUIUtility.DrawCurveSwatch(curveRect, enemyCount, null, Color.white, overlayColor);
        EditorGUIUtility.DrawCurveSwatch(curveRect, goldCount, null, Color.yellow, overlayColor);
        EditorGUIUtility.DrawCurveSwatch(curveRect, healthCount, null, Color.green, overlayColor);


        Vector2 pointSize = new Vector2(40f, 20f);
        DrawPoints(curveRect, healthCount, pointSize, new Color(Color.green.r, Color.green.g, Color.green.b, 0.1f));
        DrawPoints(curveRect, goldCount, pointSize, new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.1f));
        DrawPoints(curveRect, enemyCount, pointSize, new Color(Color.white.r, Color.white.g, Color.white.b, 0.1f));
        /*
        DrawPoint(curveRect, enemyCount, 0,0, pointSize, Color.magenta);
        DrawPoint(curveRect, enemyCount, 1,0, pointSize, Color.magenta);
        DrawPoint(curveRect, enemyCount, 0, 1, pointSize, Color.cyan);
        DrawPoint(curveRect, enemyCount, 1, 1, pointSize, Color.cyan);
        */
    }

    public static void DrawPoints(Rect curveRect, AnimationCurve curve, Vector2 pointSize, Color color)
    {
        //for (int i = 0; i < curve.keys.Length; i++)
        //for (int j = 0; j < curve.keys.Length; j++)
        //DrawPoint(curveRect, curve, i, j, pointSize, color);
        for (int i = 0; i < curve.keys.Length; i++)
            DrawPoint(curveRect, curve, i, i, pointSize, color);
    }

    public static void DrawPoint(Rect curveRect, AnimationCurve curve, int timeIndex, int valueIndex, Vector2 pointSize, Color color)
    {
        GUIStyle boxTest = EditorLabelUtilities.GetNewStyle(color);
        Rect anchoredRect = new Rect(curveRect.x, curveRect.y + curveRect.height, curveRect.width, curveRect.height);


        float time = curve.keys[timeIndex].time;
        float value = curve.keys[valueIndex].value;
        float timeLerpPoint = Mathf.InverseLerp(curve.keys.First().time, curve.keys.Last().time, time);
        float valueLerpPoint = Mathf.InverseLerp(curve.keys.First().value, curve.keys.Last().value, value);
        float lerpedX = Mathf.Lerp(anchoredRect.x, anchoredRect.x + anchoredRect.width, timeLerpPoint);
        float lerpedY = Mathf.Lerp(anchoredRect.y, anchoredRect.y - anchoredRect.height, valueLerpPoint);

        anchoredRect.x = lerpedX;
        anchoredRect.y = lerpedY;
        anchoredRect = new Rect(anchoredRect.x - (pointSize.x / 2), anchoredRect.y - (pointSize.y / 2), pointSize.x, pointSize.y);

        GUIStyle textStyle = new GUIStyle(EditorStyles.boldLabel);
        textStyle.normal.textColor = color;
        GUI.DrawTexture(anchoredRect, boxTest.normal.background);
        GUI.Label(anchoredRect, timeIndex + "," + valueIndex);
    }

    public static void DrawSpawnInfoListing(SerializedProperty spawnInfoProp, Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty spawnInfo = spawnInfoProp;
        DynamicRect dynamicRect = new DynamicRect(rect, 1f, 0f, 5f, 0f, 0f, EditorGUIUtility.singleLineHeight);
        dynamicRect.Modify(y: EditorGUIUtility.standardVerticalSpacing);
        EditorGUI.SelectableLabel(dynamicRect.Get(100), "Spawn Info #" + index + ":");
        DrawProperty(spawnInfo, dynamicRect, "Time:", nameof(SpawnInfo.TimeToSpawn), 35, 45);
        DrawProperty(spawnInfo, dynamicRect, "Enemy:", nameof(SpawnInfo.EnemyToSpawn), 45, 220);
        DrawProperty(spawnInfo, dynamicRect, "Amount:", nameof(SpawnInfo.AmountToSpawn), 50, 30);
        DrawProperty(spawnInfo, dynamicRect, "Interval:", nameof(SpawnInfo.IntervalBetweenSpawns), 50, 30);
    }

    private static void DrawProperty(SerializedProperty prop, DynamicRect rect, string header, string seekName, float hWidth, float fWidth)
    {
        EditorGUI.SelectableLabel(rect.Get(hWidth), header);
        EditorGUI.PropertyField(rect.Get(fWidth), EditorUtilities.Seek(prop, seekName), GUIContent.none);
    }

    public static void DrawField(SerializedProperty prop, DynamicRect rect, string header, float hWidth, float fWidth, bool isDisabled = false, bool isInvalid = false)
    {
        if (!string.IsNullOrEmpty(header))
            EditorGUI.SelectableLabel(rect.Get(hWidth), header + ":");
        if (isDisabled)
            EditorGUI.BeginDisabledGroup(isDisabled);
        if (prop != null)
            EditorGUI.PropertyField(rect.Get(fWidth), prop, GUIContent.none);
        if (isDisabled)
            EditorGUI.EndDisabledGroup();
    }

    public static void DrawField<T>(UnityEngine.Object objectValue, DynamicRect rect, string header, float hWidth, float fWidth, bool isDisabled = false, bool isInvalid = false) where T : UnityEngine.Object
    {
        if (!string.IsNullOrEmpty(header))
            EditorGUI.SelectableLabel(rect.Get(hWidth), header + ":");
        if (isDisabled)
            EditorGUI.BeginDisabledGroup(isDisabled);
        if (objectValue != null)
            EditorGUI.ObjectField(rect.Get(fWidth), objectValue, typeof(T), false);
        if (isDisabled)
            EditorGUI.EndDisabledGroup();
    }

    public static T DrawField<T>(T value, DynamicRect rect, string header, float hWidth, float fWidth, bool isDisabled = false, bool isInvalid = false)
    {
        if (!string.IsNullOrEmpty(header))
            EditorGUI.SelectableLabel(rect.Get(hWidth), header + ":");
        if (isDisabled)
            EditorGUI.BeginDisabledGroup(isDisabled);
        Rect valueRect = rect.Get(fWidth);

        GUIStyle validStyle = GetFieldStyle();
        if (isInvalid)
            validStyle = EditorLabelUtilities.GetNewStyle(new Color(0.05f, 0f, 0f, 1f));
        switch (value)
        {
            case float floatValue:
                float returnFloat = EditorGUI.FloatField(valueRect, floatValue, validStyle);
                if (returnFloat is T returnFloatValue)
                {
                    if (isDisabled)
                        EditorGUI.EndDisabledGroup();
                    return (returnFloatValue);
                }
                break;
            case int intValue:
                int returnInt = EditorGUI.IntField(valueRect, intValue, validStyle);
                if (returnInt is T returnIntValue)
                {
                    if (isDisabled)
                        EditorGUI.EndDisabledGroup();
                    return (returnIntValue);
                }
                break;
            case bool boolValue:
                bool returnBool = EditorGUI.Toggle(valueRect, boolValue);
                if (returnBool is T returnBoolValue)
                {
                    if (isDisabled)
                        EditorGUI.EndDisabledGroup();
                    return (returnBoolValue);
                }
                break;
            case string stringValue:
                EditorGUI.SelectableLabel(valueRect, stringValue, validStyle);
                break;
        }
        if (isDisabled)
            EditorGUI.EndDisabledGroup();

        return (default);
    }

    private static GUIStyle fieldStyle;
    public static GUIStyle GetFieldStyle()
    {
        if (fieldStyle == null)
        {
            fieldStyle = new GUIStyle(EditorStyles.textField);
            fieldStyle.fontStyle = FontStyle.Bold;
        }
        return (fieldStyle);
    }

    public static string Decorate(string name) => "<" + name + ">k__BackingField";

    public static SerializedProperty Seek(SerializedObject target, string targetField) => Seek(target.GetIterator(), targetField);

    public static SerializedProperty Seek(SerializedProperty target, string targetField)
    {
        foreach (SerializedProperty sp in EditorLabelUtilities.FindSerializedProperties(target.Copy()))
            if (sp.name.Contains(targetField) || sp.name.Contains(Decorate(targetField)))
                return (sp.Copy());
        return (null);
    }
}
