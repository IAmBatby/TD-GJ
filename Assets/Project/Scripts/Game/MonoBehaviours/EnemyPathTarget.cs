using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathTarget : MonoBehaviour
{
    public void ForwardedTriggerEnter(Collider other)
    {
        Debug.Log("Forward Enter: " + other.gameObject.name);
        if (other.TryGetComponent(out EnemyBehaviour enemy))
        {
            GameManager.EnemyReachedTarget(enemy);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, transform.localScale);
        Gizmos.color = new Color(Color.red.r, Color.red.g, Color.red.b, 0.5f);
        Gizmos.DrawCube(Vector3.zero, transform.localScale);
    }
}
