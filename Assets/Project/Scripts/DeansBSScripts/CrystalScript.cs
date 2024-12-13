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

    public void Start()
    {
        healthControl.ModifyHealth(-100000);
    }

    public void DropCrystal()
    {
        if (healthControl.Health == 0) return;

        if(listOfCrystals.Count > 0 && (healthControl.Health/healthControl.maxHealth * 100) <= DropThresholdPercent)
        {
            var crystal = listOfCrystals[0];

            if (crystal.TryGetComponent(out Animator crystalAnimator))
            {
                crystal.GetComponent<Animator>().SetTrigger("DropCrystal");
                crystal.GetComponent<Animator>().StopPlayback();
            }
            listOfCrystals.Remove(crystal);
            crystal.transform.parent = null;


            //healthControl.ResetHealth();
        }
        if (listOfCrystals.Count == 0)
            healthControl.ModifyHealth(-100000);
    }

    public void SpawnCrystal()
    {
        if(listOfCrystals.Count != 3)
        {
            if (spawnChance >= Random.Range(1,100))
            {
                var temp = Random.Range(0, listOfSpawners.Count);
                ItemBehaviour crystal = listOfSpawners[temp].Spawn();
                listOfCrystals.Add(crystal.gameObject);
                crystal.transform.position = listOfSpawners[temp].transform.position;
                healthControl.ResetHealth();
            }
        }
    }
}
