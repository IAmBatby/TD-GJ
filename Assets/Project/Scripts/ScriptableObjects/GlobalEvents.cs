using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GlobalEvents : MonoBehaviour
{
    public UnityEvent OnNewWave;
    public UnityEvent<EnemyAI> OnEnemyKilled;
    public UnityEvent<EnemyAI> OnEnemySpawned;
    public UnityEvent<ItemBehaviour> OnItemPickup;
    public UnityEvent<ItemBehaviour> OnItemDropped;

    private void Awake()
    {
        GameManager.Instance.OnNewWave.AddListener(OnNewWave.Invoke);
        GameManager.Instance.OnEnemySpawned.AddListener(OnEnemySpawned.Invoke);
        GameManager.Instance.OnEnemyKilled.AddListener(OnEnemyKilled.Invoke);
        GameManager.Player.OnItemPickup.AddListener(OnItemPickup.Invoke);
        GameManager.Player.OnItemDropped.AddListener(OnItemDropped.Invoke);
    }
}
