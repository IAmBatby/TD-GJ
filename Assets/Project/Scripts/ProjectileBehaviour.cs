using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private AudioSource primarySource;
    [field: SerializeField] public ScriptableProjectile Data { get; private set; }

    [SerializeField] private bool doesPierce;
    private Vector3 targetPosition;
    private int damage;

    [SerializeField] private float lifeTimer;
    private Timer killTimer;

    public void Initialize(ScriptableProjectile newData, Vector3 newTargetPosition, float newForce, int newDamage)
    {
        Data = newData;
        targetPosition = newTargetPosition;
        damage = newDamage;

        rigidBody.AddForce((targetPosition - transform.position).normalized * newForce, ForceMode.Impulse);
        transform.LookAt(newTargetPosition);

        killTimer = new Timer();
        killTimer.onTimerEnd.AddListener(DestroyBullet);
        killTimer.StartTimer(this, lifeTimer);
    }

    public void ForwardedTriggerEnter(Collider other)
    {
        HealthController health = other.transform.root.GetComponentInChildren<HealthController>();
        if (health != null)
        {
            AudioManager.PlayAudio(Data.OnCollisionAudioPreset, primarySource);
            health.ModifyHealth(-damage);
            if (!doesPierce)
            {
                DestroyBullet();
            }
        }
    }

    public void DestroyBullet()
    {
        gameObject.SetActive(false);
        GameObject.Destroy(gameObject);
    }
}
