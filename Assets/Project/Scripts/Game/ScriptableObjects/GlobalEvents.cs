using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GlobalEvents : MonoBehaviour
{
    public UnityEvent OnNewWave;
    public UnityEvent<EnemyBehaviour> OnEnemyKilled;
    public UnityEvent<EnemyBehaviour> OnEnemySpawned;
    public UnityEvent<ItemBehaviour> OnItemPickup;
    public UnityEvent<ItemBehaviour> OnItemDropped;

    private void OnEnable() => GameManager.OnGameManagerStart.AddListener(Initialize);

    private void Initialize()
    {
        GameManager.OnNewWave.AddListener(OnNewWave.Invoke);
        GameManager.OnEnemySpawned.AddListener(OnEnemySpawned.Invoke);
        GameManager.OnEnemyKilled.AddListener(OnEnemyKilled.Invoke);
        GameManager.Player.OnItemPickup.AddListener(OnItemPickup.Invoke);
        GameManager.Player.OnItemDropped.AddListener(OnItemDropped.Invoke);
    }
}
