using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldDropScript : MonoBehaviour
{
    public ContentSpawner spawner;
    public int chanceNeeded;
    public int minRange;
    public int maxRange;
    
    public void DropMoneyChance(EnemyBehaviour enemy)
    {
        if(chanceNeeded >= Random.Range(minRange, maxRange))
        {
            var temp = spawner.Spawn();
            temp.transform.position = enemy.transform.position;
        }
    }
}
