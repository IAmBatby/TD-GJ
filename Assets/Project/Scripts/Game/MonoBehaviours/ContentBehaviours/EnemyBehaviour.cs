using IterationToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyBehaviour : HurtableBehaviour
{
    public ScriptableEnemy EnemyData { get; private set; }
    protected NavMeshAgent Agent { get; private set; }
    private EnemyPathTarget target;

    public float RemainingDestinationDistance
    {
        get
        {
            if (Agent == null || (Agent != null && Agent.hasPath == false))
                return (Mathf.Infinity);
            else
                return (Agent.remainingDistance);
        }
    }

    private void OnEnable()
    {
        Agent = GetComponent<NavMeshAgent>();
        if (Agent == null)
            Debug.LogError("Failed To Get NavMeshAget", transform);
    }

    protected override void OnSpawn()
    {
        base.OnSpawn();
        if (ContentData is ScriptableEnemy enemyData)
            EnemyData = enemyData;
        if (EnemyData.Speed != 0)
            Agent.speed = Utilities.GetScaledValue(EnemyData.Speed, EnemyData.SpeedWaveScale, GameManager.CurrentWaveCount);
    }

    public void RecieveNewTarget(EnemyPathTarget newTarget)
    {
        enabled = true;
        target = newTarget;
        Agent.Warp(transform.position);
    }

    private void Update()
    {
        if (target == null) return;
        if (enabled == false) return;

        //Im sorry
        try
        {
            Agent.SetDestination(target.transform.position);
        }
        catch
        {

        }

    }

    public void SetAvoidancePriority(int newValue)
    {
        if (Agent == null) return;
        Agent.avoidancePriority = newValue;
    }

    protected override void OnDeath()
    {
        Agent.enabled = false;
        GameManager.RemoveEnemy(this);
    }

    private void ForwardedTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerBehaviour player))
        {
            player.ModifyHealth(EnemyData.Damage);
        }
    }

    private void OnDrawGizmos()
    {
        /*
        if (!GameManager.Instance.AllSpawnedEnemies.Contains(this))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, Vector3.one);
        }
        */
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
