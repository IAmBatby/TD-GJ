using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableWave", menuName = "TD-GJ/ScriptableWave", order = 1)]
public class ScriptableWave : ScriptableObject
{
    [field: SerializeField] public float IntermissionTimeLength { get; private set; }
    [field: SerializeField] public List<SpawnInfo> WaveSpawnInfos { get; private set; } = new List<SpawnInfo>();
    [SerializeField] public int fakeWaveIndex = 1;

    public List<(ScriptableEnemy, float)> GetEnemySpawnManifest()
    {
        List<(ScriptableEnemy, float)> returnList = new List<(ScriptableEnemy, float)>();
        foreach (SpawnInfo enemyInfo in WaveSpawnInfos)
        {
            if (enemyInfo.EnemyToSpawn == null) continue;
            for (int i = 0; i < enemyInfo.AmountToSpawn; i++)
                returnList.Add((enemyInfo.EnemyToSpawn, enemyInfo.TimeToSpawn + (enemyInfo.IntervalBetweenSpawns * i)));
        }

        returnList = returnList.OrderBy(e => e.Item2).ToList();
        return (returnList);
    }

    public bool IsValidWave()
    {
        if (GetTotalEnemyCount() > 0 && GetFinalTime() > 0)
            return (true);
        else
            return (false);
    }

    public float GetFinalTime()
    {
        float highestTimeToSpawnTime = 0f;
        foreach (SpawnInfo spawnInfo in WaveSpawnInfos)
            if (spawnInfo.IsValidSpawnRequest() && spawnInfo.TimeToSpawn > highestTimeToSpawnTime)
                highestTimeToSpawnTime = spawnInfo.TimeToSpawn;
        return (highestTimeToSpawnTime);
    }

    public float GetTotalTime()
    {
        List<(ScriptableEnemy enemy, float)> manifest = GetEnemySpawnManifest();
        return (manifest.Count > 0 ? manifest.Last().Item2 : 0f);
    }

    public int GetTotalEnemyCount()
    {
        int enemyCount = 0;
        foreach (KeyValuePair<ScriptableEnemy, int> spawnInfoEntry in GetEnemyCountDict())
            enemyCount += spawnInfoEntry.Value;
        return (enemyCount);
    }

    public float GetTotalEnemyHealth(int waveIndex)
    {
        float totalDamage = 0f;
        foreach (KeyValuePair<ScriptableEnemy, int> kvp in GetEnemyCountDict())
            totalDamage += GetCombinedEnemyHealth(kvp.Key, waveIndex);
        return (totalDamage);
    }

    public float GetCombinedEnemyHealth(ScriptableEnemy enemy, int waveIndex)
    {
        Dictionary<ScriptableEnemy, int> kvp = GetEnemyCountDict();
        if (kvp.TryGetValue(enemy, out int count))
            return (Utilities.GetScaledValue(enemy.Health * count, enemy.HealthWaveScale, waveIndex));
        else
            return (-1f);
    }

    public Vector2 GetTotalEnemyGold(int waveIndex)
    {
        Vector2 totalGold = Vector2.zero;
        foreach (KeyValuePair<ScriptableEnemy, int> kvp in GetEnemyCountDict())
            totalGold += GetCombinedEnemyGold(kvp.Key, waveIndex);
        return (totalGold);
    }

    public Vector2 GetCombinedEnemyGold(ScriptableEnemy enemy, int waveIndex)
    {
        if (enemy.ItemDrop != null && enemy.ItemDrop is ScriptableCurrency currency)
        {
            Dictionary<ScriptableEnemy, int> kvp = GetEnemyCountDict();
            if (kvp.TryGetValue(enemy, out int count))
                return (Utilities.GetScaledValue(new Vector2(currency.CurrencyAmount.x * count, currency.CurrencyAmount.y * count), currency.CurrencyWaveScale, waveIndex));
        }
        return (Vector2.zero);
    }

    //Awful
    public Dictionary<ScriptableEnemy, int> GetEnemyCountDict()
    {
        Dictionary<ScriptableEnemy, int> enemyDict = new Dictionary<ScriptableEnemy, int>();
        foreach (SpawnInfo info in WaveSpawnInfos)
        {
            if (info.EnemyToSpawn != null)
            {
                if (!enemyDict.ContainsKey(info.EnemyToSpawn))
                    enemyDict.Add(info.EnemyToSpawn, 0);
                enemyDict[info.EnemyToSpawn] += info.AmountToSpawn;
            }
        }
        List<(ScriptableEnemy, int)> orderedList = new List<(ScriptableEnemy, int)>();
        foreach (KeyValuePair<ScriptableEnemy, int> kvp in enemyDict)
            orderedList.Add((kvp.Key, kvp.Value));

        orderedList = orderedList.OrderBy(o => o.Item2).Reverse().ToList();
        enemyDict.Clear();
        foreach ((ScriptableEnemy, int) orderedKvp in orderedList)
            enemyDict.Add(orderedKvp.Item1, orderedKvp.Item2);

        return (enemyDict);
    }
}
