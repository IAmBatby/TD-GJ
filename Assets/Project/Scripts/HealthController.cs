using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    [SerializeField] public int maxHealth;
    private int currentHealth;

    public int Health { get => currentHealth; set => ModifyHealth(value); }

    [SerializeField] private Transform mainParent;
    [SerializeField] private AudioSource onHealthLostAudioSource;
    [SerializeField] private Image healthFillAmountImage;
    [SerializeField] private TextMeshProUGUI healthText;

    private AudioPreset onHealthLostPreset;


    public ExtendedEvent<(int, int)> OnHealthModified = new ExtendedEvent<(int, int)>();
    public ExtendedEvent OnDeath = new ExtendedEvent<(int, int)>();

    public MonoBehaviour LinkedBehaviour { get; private set; }

    private void OnEnable() => GameManager.Instance.AllHealthControllers.Add(this);
    private void OnDisable() => GameManager.Instance.AllHealthControllers.Remove(this);


    private void Awake()
    {
        currentHealth = maxHealth;
        OnHealthModified.AddListener(RefreshUI);
        RefreshUI();
    }

    private void Update()
    {
        mainParent.gameObject.SetActive(!GameManager.Instance.HasGameEnded);
    }

    private void LateUpdate()
    {
        mainParent.transform.LookAt(GameManager.Player.ActiveCamera.transform.position);
    }

    public void ModifyHealth(int value)
    {
        int previousHealth = currentHealth;

        currentHealth += value;

        if (currentHealth < 0)
            currentHealth = 0;

        if (currentHealth == 0)
            OnDeath.Invoke();

        if (currentHealth < previousHealth && onHealthLostPreset != null)
            AudioManager.PlayAudio(onHealthLostPreset, onHealthLostAudioSource);

        OnHealthModified.Invoke((previousHealth, currentHealth));
    }

    public void SetUIActive(bool value)
    {
        mainParent.gameObject.SetActive(value);
    }

    private void DisableUI() => SetUIActive(false);
    private void EnableUI() => SetUIActive(true);

    public void LinkBehaviour(MonoBehaviour behaviour, AudioPreset healthLostPreset)
    {
        LinkedBehaviour = behaviour;
        onHealthLostPreset = healthLostPreset;

        if (behaviour is ItemBehaviour itemBehaviour)
        {
            itemBehaviour.OnMouseoverToggle.AddListener(EnableUI);
            itemBehaviour.OnMouseoverToggle.AddListener(DisableUI);
        }
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
    }    

    public void ResetHealth()
    {
        int previousHealth = currentHealth;

        currentHealth = maxHealth;

        OnHealthModified.Invoke((previousHealth, currentHealth));
    }

    private void RefreshUI()
    {
        healthText.SetText(currentHealth.ToString());
        healthFillAmountImage.fillAmount = Mathf.InverseLerp(0, maxHealth, currentHealth);
    }
}
