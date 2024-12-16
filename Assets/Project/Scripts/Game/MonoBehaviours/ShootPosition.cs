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
}
