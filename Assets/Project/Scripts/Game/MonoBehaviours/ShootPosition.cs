using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ShootPosition : MonoBehaviour
{
    private enum ShootDirectionMode { Target, Forward }
    [SerializeField] private ShootDirectionMode shootMode;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LayerMask enemyMask;

    private void OnEnable()
    {
        lineRenderer.enabled = false;
    }

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
        Vector3 targetPos;
        if (shootMode == ShootDirectionMode.Target)
            targetPos = GetAccuracyPosition(target.transform, accuracy);
        else
            targetPos = transform.position + transform.forward;
        projectile.SpawnProjectile(transform.position, targetPos, shotSpeed, damage);
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


    private void OnDrawGizmos()
    {
        if (lineRenderer != null && GlobalData.Instance == null) //hack to see if were not playing
        {
            IterationToolkit.Utilities.DrawLabel(transform.position + new Vector3(0, 0, 5), "FakeTarget", Color.red);
            if (shootMode == ShootDirectionMode.Target)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, transform.position + new Vector3(0, 0, 5));
            }
            else if (shootMode == ShootDirectionMode.Forward)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, transform.position + (transform.forward * 5));
            }
        }
    }
}
