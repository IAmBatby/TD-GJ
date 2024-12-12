using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopScript : MonoBehaviour
{
    public ItemSpawner spawner;
    public ScriptableItem coin;

    public void ToggleSpawnEatMoney(Collider other)
    {
        if (other.TryGetComponent(out ItemBehaviour item))
            if (item.ItemData == coin) ;
    }

}
