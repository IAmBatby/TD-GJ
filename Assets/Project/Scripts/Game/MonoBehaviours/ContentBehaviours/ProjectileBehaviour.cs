using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ProjectileBehaviour : ContentBehaviour
{
    public ScriptableProjectile ProjectileData { get; private set; }
    
    private Timer killTimer;
    private List<GameObject> piercedObjects = new List<GameObject>();

    [SerializeField] private LayerMask hitMask;

    private Vector3 targetPosition;
    private float force;
    private int damage;

    private bool hasDespawned;
    protected override void OnSpawn()
    {
        base.OnSpawn();
        if (ContentData is ScriptableProjectile projData)
            ProjectileData = projData;

        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.material.color = ProjectileData.ProjectileColor;
            renderer.material.SetColor("_EmissionColor", ProjectileData.ProjectileColor);
        }
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
        HurtableBehaviour health = other.transform.root.GetComponentInChildren<HurtableBehaviour>();
        if (health != null)
            health.ModifyHealth(-damage);
        if (ProjectileData.IsPiercing)
            piercedObjects.Add(other.gameObject);

        Explode();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Enemy")) //Bad
            Explode();
    }

    public virtual void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, ProjectileData.ImpactExplosionRadius, hitMask, QueryTriggerInteraction.Collide);
        List<HurtableBehaviour> behaviours = new List<HurtableBehaviour>();
        foreach (Collider hit in hits)
        {
            if (piercedObjects.Contains(hit.transform.gameObject)) continue;
            foreach (HurtableBehaviour behaviour in hit.transform.gameObject.GetComponentsInChildren<HurtableBehaviour>())
                if (!behaviours.Contains(behaviour))
                    behaviours.Add(behaviour);
        }

        Debug.Log("Cannonball Exploding! Radius Is: " + ProjectileData.ImpactExplosionRadius + ", Hit: #" + behaviours.Count + "Hurtables");

        foreach (HurtableBehaviour behaviour in  behaviours)
            behaviour.ModifyHealth(-damage);


        ReactionPlayer.Audio.PlayAudio(ProjectileData.OnCollisionAudio);
        ReactionPlayer.Particles.PlayParticle(ProjectileData.OnCollisionParticle);
        //Debug.Log("Particle Played!", transform);
        //Debug.Break();

        if (ProjectileData.IsPiercing)
        {
            foreach (Collider hit in hits)
                if (!piercedObjects.Contains(hit.transform.gameObject))
                    piercedObjects.Add(hit.transform.gameObject);
        }
        else
            Despawn();
    }

    public void Despawn()
    {
        if (hasDespawned) return;
        ReactionPlayer.Particles.DetatchParticles();
        GameManager.UnregisterContentBehaviour(this, true);
        hasDespawned = true;
    }

    public override void RegisterBehaviour()
    {
        ContentManager.RegisterBehaviour(this);
        base.RegisterBehaviour();
    }
    public override void UnregisterBehaviour(bool destroyOnUnregistration)
    {
        ContentManager.UnregisterBehaviour(this, destroyOnUnregistration);
        base.UnregisterBehaviour(destroyOnUnregistration);
    }
}
