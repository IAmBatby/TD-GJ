using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SavedWaveInfo
{
    public ScriptableWave Wave { get; private set; }
    public int WaveIndex { get; private set; }
    public float FinalTime { get; private set; }
    public float TotalTime { get; private set; }
    public Dictionary<ScriptableEnemy, int> Enemies { get; private set; }
    public int EnemyCount { get; private set; } 
    public float TotalHealth { get; private set; }
    public Vector2 TotalGold { get; private set; }
    public bool IsValid { get; private set; }

    public SavedWaveInfo(ScriptableWave wave, int waveIndex)
    {
        Wave = wave;
        WaveIndex = waveIndex;
        FinalTime = wave.GetFinalTime();
        TotalTime = wave.GetTotalTime();
        Enemies = wave.GetEnemyCountDict();
        EnemyCount = wave.GetTotalEnemyCount();
        TotalHealth = wave.GetTotalEnemyHealth(WaveIndex);
        TotalGold = wave.GetTotalEnemyGold(WaveIndex);
        IsValid = wave.IsValidWave();
    }
}
