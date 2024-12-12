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
    private EnemyPathTarget target;

    private int currentHealth;
    public int Health { get => currentHealth; set => ModifyHealth(value); }

    private void Awake()
    {
        enabled = false;
    }

    public void InitializeEnemy(ScriptableEnemy newData, EnemyPathTarget newTarget)
    {
        Data = newData;
        target = newTarget;
        currentHealth = Data.Health;
        enabled = true;
    }

    private void Update()
    {
        Agent.SetDestination(target.transform.position);
    }

    public void RecieveHit(int value)
    {
        ModifyHealth(value);
    }

    public void ModifyHealth(int value)
    {
        currentHealth += value;

        if (currentHealth < 0)
            currentHealth = 0;

        if (currentHealth == 0)
            Die();

    }

    public void Die()
    {
        Agent.enabled = false;
        GameManager.RemoveEnemy(this);
    }
}
