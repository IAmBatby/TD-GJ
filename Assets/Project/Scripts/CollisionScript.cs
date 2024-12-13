using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionScript : MonoBehaviour
{
    ProjectileBehaviour projectile;


    private void OnCollisionEnter(Collision collision)
    {
        GameObject.Destroy(projectile);
    }
}
