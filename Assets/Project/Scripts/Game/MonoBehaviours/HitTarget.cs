using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HitTarget : MonoBehaviour
{
    [SerializeField] private int MaxHealth;
    private int currentHealth;
    public int Health { get => currentHealth; set => ModifyHealth(value); }

    public Image healthFillImage;
    public TextMeshProUGUI healthText;


    public void ModifyHealth(int value)
    {

    }
}
