using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalScript : MonoBehaviour, IHittable
{
    public ItemSpawner spawner;
    public List<GameObject> listOfCrystals = new List<GameObject>();

    public bool drop;
    public Transform GetTransform()
    {
        throw new System.NotImplementedException();
    }

    public void RecieveHit(int value)
    {
        DropCrystal();
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        if (drop)
        {
            drop = false;
            DropCrystal();
        }
    }
    public void DropCrystal()
    {
        if(listOfCrystals.Count > 0)
        {
            var crystal = listOfCrystals[0];

            crystal.GetComponent<Animator>().SetTrigger("DropCrystal");
            crystal.GetComponent<Animator>().StopPlayback();
            listOfCrystals.Remove(crystal);
            crystal.transform.parent = null;
        }
    }
}
