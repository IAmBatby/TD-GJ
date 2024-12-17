using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldDropBehaviour : ItemBehaviour
{
    private int currencyValue;


    protected override void OnSpawn()
    {
        base.OnSpawn();
        if (ContentData is ScriptableCurrency currencyData)
        {
            float randomGold = Random.Range(currencyData.CurrencyAmount.x, currencyData.CurrencyAmount.y);
            currencyValue = Mathf.RoundToInt(Utilities.GetScaledValue(randomGold, currencyData.CurrencyWaveScale, GameManager.Instance.CurrentWaveCount));
        }
        GameManager.OnNewWave.AddListener(DestroyCoin);
    }
    public void ForwardedTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerBehaviour player))
        {
            if(!IsBeingHeld)
            {
                player.PickupItem(this);
                player.DropItem(Vector3.zero);
                GameManager.ModifyCurrency(currencyValue);

                GameManager.UnregisterContentBehaviour(this, true);
            }
        }
    }

    public void DestroyCoin()
    {
        GameManager.OnNewWave.RemoveListener(DestroyCoin);
        GameManager.UnregisterContentBehaviour(this, true);
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
