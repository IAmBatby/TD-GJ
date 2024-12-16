using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ShootPosition : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

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
        float offset = 2.5f; //hardcoded rn

        offset = Random.Range(0, accuracy);

        Vector3 rushingPosition = targetTransform.position + (targetTransform.forward + new Vector3(offset,0,0));
        Vector3 draggingPosition = targetTransform.position + (targetTransform.forward + new Vector3(-offset, 0, 0));

        return (Vector3.Lerp(rushingPosition, draggingPosition, 0.5f));
    }
}
