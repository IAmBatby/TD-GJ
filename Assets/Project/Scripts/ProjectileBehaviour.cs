using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidBody;
    [field: SerializeField] public ScriptableProjectile Data { get; private set; }

    private Vector3 targetPosition;
    private int damage;

    public void Initialize(ScriptableProjectile newData, Vector3 newTargetPosition, float newForce, int newDamage)
    {
        Data = newData;
        targetPosition = newTargetPosition;
        damage = newDamage;

        rigidBody.AddForce((targetPosition - transform.position).normalized * newForce, ForceMode.Impulse);
    }

    public void ForwardedTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IHittable hittable))
        {
            hittable.RecieveHit(-damage);
            gameObject.SetActive(false);
            GameObject.Destroy(gameObject);
        }
    }
}
