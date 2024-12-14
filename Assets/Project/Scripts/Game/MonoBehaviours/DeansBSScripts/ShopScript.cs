using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopScript : MonoBehaviour
{
    public ContentSpawner spawner;
    public ScriptableItem coin;
    public Animator anim;

    public SpriteRenderer spriteRenderer1;
    public SpriteRenderer spriteRenderer2;

    public void Awake()
    {
        if(spawner.DefaultContent is ScriptableUpgrade upgrade)
        {
            spriteRenderer1.sprite = upgrade.Attribute.DisplayIcon;
            spriteRenderer2.sprite = upgrade.Attribute.DisplayIcon;

            //spriteRenderer1.color = upgrade.Attribute.DisplayColor;
            //spriteRenderer2.color = upgrade.Attribute.DisplayColor;
        }
    }
    public void ToggleSpawnEatMoney(Collider other)
    {
        if (other.TryGetComponent(out PlayerBehaviour player))
        {
            anim.SetBool("PlayerisNear", true);
        }
        else if (other.TryGetComponent(out ItemBehaviour item))
            if (item.ItemData == coin)
            {
                anim.SetTrigger("CoinDeposited");
                other.enabled = false;
                GameObject.Destroy(other.gameObject);
                spawner.Spawn();
            }
    }

    public void ForwardedTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerBehaviour player))
        {
            anim.SetBool("PlayerisNear", false);
        }
    }

}
