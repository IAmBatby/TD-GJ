using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldDropBehaviour : ItemBehaviour
{
    private ScriptableCurrency CurrencyData;
    private int currency;

    protected override void OnSpawn()
    {
        base.OnSpawn();
        if (ContentData is ScriptableCurrency currencyData)
            CurrencyData = currencyData;
        Vector2 scaledAmount = GameManager.WaveManifest.GetAdditiveGold(CurrencyData.CurrencyAmount, GameManager.CurrentWaveCount);
        currency = Mathf.RoundToInt(Random.Range(scaledAmount.x, scaledAmount.y));
        GeneralDisplayInfo.DisplayMode = DisplayType.Mini;
        GeneralDisplayInfo.SetDisplayValues(string.Empty);
        //GeneralDisplayInfo.SetDisplayValues("$" + currency.ToString());
        //GeneralDisplayInfo.DisplayIcon = null;
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
                GameManager.ModifyCurrency(currency);
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
