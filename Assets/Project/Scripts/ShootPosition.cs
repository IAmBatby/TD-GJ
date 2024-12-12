using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ShootPosition : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    public void UpdateShootRenderer(IHittable target)
    {
        if (target != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;

            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, target.GetTransform().position);
        }
        else
            lineRenderer.enabled = false;
    }
}
