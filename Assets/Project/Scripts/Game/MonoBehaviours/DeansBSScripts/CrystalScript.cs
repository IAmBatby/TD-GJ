using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalScript : MonoBehaviour
{
    public List<ContentSpawner> listOfSpawners = new List<ContentSpawner>();
    public List<GameObject> listOfCrystals = new List<GameObject>();

    public HurtableBehaviour healthControl;
    public int DropThresholdPercent;
    public int spawnChance;

    public void Start()
    {
        healthControl.ModifyHealth(-100000);
    }

    public void DropCrystal()
    {
        if (healthControl.Health == 0 || healthControl.Health == healthControl.MaxHealth) return;
        if(listOfCrystals.Count != 0 && (healthControl.Health/healthControl.MaxHealth * 100) <= DropThresholdPercent)
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
        if(listOfCrystals.Count != listOfSpawners.Count)
        {
            if (spawnChance >= Random.Range(1,100))
            {
                var temp = Random.Range(0, listOfSpawners.Count);
                ItemBehaviour crystal = listOfSpawners[temp].Spawn() as ItemBehaviour;
                listOfCrystals.Add(crystal.gameObject);
                crystal.transform.position = listOfSpawners[temp].transform.position;
                healthControl.ResetHealth();
            }
        }
    }
}
