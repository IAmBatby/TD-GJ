using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurretStatElement : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private ScriptableAttribute trackingAttribute;

    private void Awake()
    {
        OnMouseoverToggle(false);
    }

    public void SetAttribute(ScriptableAttribute newTracking)
    {
        trackingAttribute = newTracking;
    }

    private void Update()
    {
        if (trackingAttribute == null) return;
        transform.LookAt(GameManager.Player.ActiveCamera.transform.position);

        if (trackingAttribute != null)
        {
            iconImage.sprite = trackingAttribute.DisplayIcon;
            backgroundImage.color = trackingAttribute.DisplayColor;
            if (trackingAttribute is ScriptableFloatAttribute floatAttribute)
            {
                float attributeValue = floatAttribute.GetAttributeValue();
                float defaultValue = floatAttribute.GetDefaultValue();
                valueText.SetText(attributeValue.ToString());
                if (attributeValue > defaultValue)
                {
                    iconImage.color = Color.yellow;
                    valueText.color = Color.yellow;
                }


            }
        }
    }

    public void OnMouseoverToggle(bool value)
    {
        iconImage.enabled = value;
        backgroundImage.enabled = value;
        valueText.enabled = value;
    }
}
