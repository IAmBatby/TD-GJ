using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeBehaviour : ItemBehaviour
{
    public ScriptableAttribute Attribute { get; private set; }
    public float DefaultValue { get; private set; }

    [SerializeField] private SpriteRenderer upgradeSpriteFront;
    [SerializeField] private SpriteRenderer upgradeSpriteBack;

    protected override void OnSpawn()
    {
        base.OnSpawn();
        if (ItemData is ScriptableUpgrade upgradeData)
        {
            Attribute = upgradeData.Attribute;
            DefaultValue = upgradeData.ModifierValue;
        }    
        upgradeSpriteFront.sprite = Attribute.DisplayIcon;
        upgradeSpriteBack.sprite = Attribute.DisplayIcon;
    }

    public void ForwardedTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out TurretBehaviour turret))
        {
            if (turret.TryModifyAttribute(Attribute, DefaultValue))
            {
                if (GameManager.Player.ActiveItem == this)
                    GameManager.Player.DropItem(transform.position);
                GameManager.UnregisterContentBehaviour(this, true);
            }
        }
    }
}