using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeBehaviour : ItemBehaviour
{
    [field: SerializeField] public ScriptableAttribute Attribute;
    [field: SerializeField] public float DefaultValue;

    [SerializeField] private SpriteRenderer upgradeSpriteFront;
    [SerializeField] private SpriteRenderer upgradeSpriteBack;

    protected override void OnSpawn()
    {
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
            foreach (ScriptableAttribute turretAttribute in turret.AllAttributes)
                if (turretAttribute.Compare(Attribute))
                {
                    if (turretAttribute is ScriptableAttribute<float> floatAtr)
                        floatAtr.AddModifier(DefaultValue);
                    else if (turretAttribute is ScriptableAttribute<int> intAtr)
                        intAtr.AddModifier((int)DefaultValue);

                    if (GameManager.Player.ActiveItem == this)
                        GameManager.Player.DropItem(transform.position);
                    gameObject.SetActive(false);
                    GameObject.Destroy(gameObject);
                }
        }
    }
}
