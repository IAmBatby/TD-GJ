using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : ContentBehaviour
{
    public ScriptableProjectile ProjectileData { get; private set; }
    
    private Timer killTimer;
    private List<GameObject> piercedObjects = new List<GameObject>();

    private Vector3 targetPosition;
    private float force;
    private int damage;

    private bool hasDespawned;
    protected override void OnSpawn()
    {
        base.OnSpawn();
        if (ContentData is ScriptableProjectile projData)
            ProjectileData = projData;
    }

    public void ApplyProjectile(Vector3 newTarget, float newForce, int newDamage)
    {
        targetPosition = newTarget;
        damage = newDamage;
        force = newForce;
        Rigidbody.AddForce((targetPosition - transform.position).normalized * force, ForceMode.Impulse);
        transform.LookAt(targetPosition);
        killTimer = new Timer();
        killTimer.onTimerEnd.AddListener(Despawn);
        killTimer.StartTimer(this, ProjectileData.LifeTime);
    }

    public void ForwardedTriggerEnter(Collider other)
    {
        if (piercedObjects.Contains(other.gameObject)) return;
        HealthController health = other.transform.root.GetComponentInChildren<HealthController>();
        if (health != null)
        {
            AudioPlayer.PlayAudio(ProjectileData.OnCollisionAudio);
            health.ModifyHealth(-damage);
            if (ProjectileData.IsPiercing)
                piercedObjects.Add(other.gameObject);
            else
                Despawn();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Enemy")) //Bad
            Despawn();
    }

    public void Despawn()
    {
        if (hasDespawned) return;
        GameManager.UnregisterContentBehaviour(this, true);
        hasDespawned = true;
    }
}
