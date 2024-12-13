using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalScript : MonoBehaviour
{
    public List<ItemSpawner> listOfSpawners = new List<ItemSpawner>();
    public List<GameObject> listOfCrystals = new List<GameObject>();

    public HealthController healthControl;
    public int DropThresholdPercent;
    public int spawnChance;
    

    public void DropCrystal()
    {
        if(listOfCrystals.Count > 0 && (healthControl.Health/healthControl.maxHealth * 100) <= DropThresholdPercent)
        {
            var crystal = listOfCrystals[0];

            crystal.GetComponent<Animator>().SetTrigger("DropCrystal");
            crystal.GetComponent<Animator>().StopPlayback();
            listOfCrystals.Remove(crystal);
            crystal.transform.parent = null;

            healthControl.ResetHealth();
        }
    }

    public void SpawnCrystal()
    {
        if(listOfCrystals.Count != 3)
        {
            healthControl.ResetHealth();
            if (spawnChance >= Random.Range(1,100))
            {
                var temp = Random.Range(0, listOfSpawners.Count);
                listOfCrystals.Add(listOfSpawners[temp].Spawn().gameObject);
            }
        }
    }
}
