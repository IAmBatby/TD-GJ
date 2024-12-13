using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IHittable
{
    [field: SerializeField] public NavMeshAgent Agent { get; private set; }
    [field: SerializeField] public Rigidbody Rigidbody { get; private set; }
    [field: SerializeField] public List<Collider> AllColliders { get; private set; } = new List<Collider>();
    [field: SerializeField] public ScriptableEnemy Data { get; private set; }
    [field: SerializeField] public HealthController HealthController { get; private set; }



    private EnemyPathTarget target;

    private int currentHealth;
    public int Health { get => currentHealth; set => HealthController.ModifyHealth(value); }

    private void Awake()
    {
        enabled = false;
    }

    public void InitializeEnemy(ScriptableEnemy newData, EnemyPathTarget newTarget)
    {
        Data = newData;
        target = newTarget;
        currentHealth = Data.Health;
        if (newData.Speed != 0)
            Agent.speed = newData.Speed;
        enabled = true;
        Agent.Warp(transform.position);
        InitializeHealthBehaviour();
    }

    private void InitializeHealthBehaviour()
    {
        HealthController.LinkBehaviour(this, Data.DamageAudioPreset);
        HealthController.SetMaxHealth(Data.Health);
        HealthController.ResetHealth();
        HealthController.OnDeath.AddListener(Die);
    }

    private void Update()
    {
        Agent.SetDestination(target.transform.position);
    }

    public void RecieveHit(int value)
    {
        HealthController.ModifyHealth(value);
    }

    //Unused
    public void ModifyHealth(int value)
    {
        currentHealth += value;

        if (currentHealth < 0)
            currentHealth = 0;

        if (currentHealth == 0)
            Die();

    }

    public Transform GetTransform() => transform;

    public void Die()
    {
        GameManager.ModifyCurrency(Mathf.RoundToInt(Random.Range(Data.GoldDropRateMinMax.x, Data.GoldDropRateMinMax.y)));
        Agent.enabled = false;
        GameManager.RemoveEnemy(this);
    }

    private void OnDrawGizmos()
    {
        if (!GameManager.Instance.AllSpawnedEnemies.Contains(this))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, Vector3.one);
        }
    }
}
