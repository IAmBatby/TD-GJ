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

        char thing = DefaultValue > 0 ? '+' : '-';
        GeneralDisplayInfo.SetDisplayValues(DefaultValue.ToString());
    }

    public void ForwardedTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out TurretBaseBehaviour turret))
        {
            if (turret.TryModifyAttribute(Attribute, DefaultValue))
            {
                if (GameManager.Player.ActiveItem == this)
                    GameManager.Player.DropItem(transform.position);
                GameManager.UnregisterContentBehaviour(this, true);
            }
        }
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
