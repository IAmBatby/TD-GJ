using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ShootPosition : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private LayerMask enemyMask;

    public void UpdateShootRenderer(HurtableBehaviour target)
    {
        if (target != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, target.transform.position);
        }
    }

    public void ToggleRenderer(bool value) => lineRenderer.enabled = value;

    public void ShootAtTarget(ScriptableProjectile projectile, HurtableBehaviour target, int damage, float shotSpeed, float accuracy)
    {
        projectile.SpawnProjectile(transform.position, GetAccuracyPosition(target.transform, accuracy), shotSpeed, damage);
    }

    public Vector3 GetAccuracyPosition(Transform targetTransform, float accuracy)
    {
        float offset = Mathf.Lerp(5f, 0, accuracy); //Hardcoded rn
        Vector3 rushingPosition = targetTransform.position + (targetTransform.forward + new Vector3(offset, 0, 0));
        Vector3 draggingPosition = targetTransform.position + (-targetTransform.forward + new Vector3(-offset, 0, 0));
        if (Physics.Raycast(transform.position, targetTransform.position, out RaycastHit hit, Mathf.Infinity, enemyMask))
        {
            rushingPosition = hit.point + (hit.point + new Vector3(offset, 0, 0));
            draggingPosition = hit.point + (-hit.normal + new Vector3(-offset, 0, 0));

        }

        rushingPosition = Vector3.Lerp(rushingPosition, targetTransform.position, accuracy);
        draggingPosition = Vector3.Lerp(draggingPosition, targetTransform.position, accuracy);



        return (Vector3.Lerp(rushingPosition, draggingPosition, Random.Range(Mathf.Lerp(0,0.5f,accuracy), Mathf.Lerp(1f,0.5f,accuracy))));
    }

   
}
