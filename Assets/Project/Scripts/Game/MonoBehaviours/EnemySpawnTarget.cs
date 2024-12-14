using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnTarget : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, transform.localScale);
        Gizmos.color = new Color(Color.green.r, Color.green.g, Color.green.b, 0.5f);
        Gizmos.DrawCube(Vector3.zero, transform.localScale);
    }
}
