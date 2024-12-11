using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [field: SerializeField] public NavMeshAgent Agent { get; private set; }
    [field: SerializeField] public Rigidbody Rigidbody { get; private set; }
    [field: SerializeField] public List<Collider> AllColliders { get; private set; } = new List<Collider>();
    [field: SerializeField] public ScriptableEnemy Data { get; private set; }
    private EnemyPathTarget target;

    private void Awake()
    {
        enabled = false;
    }

    public void InitializeEnemy(ScriptableEnemy newData, EnemyPathTarget newTarget)
    {
        Data = newData;
        target = newTarget;
        enabled = true;
    }

    private void Update()
    {
        Agent.SetDestination(target.transform.position);
    }
}
