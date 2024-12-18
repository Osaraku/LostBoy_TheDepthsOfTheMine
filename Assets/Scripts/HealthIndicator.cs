using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;

public class HealthIndicator : MonoBehaviour
{
    public TMP_Text healthText;
    Damageable playerDamageable;
    private float healthPercentage;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerDamageable = player.GetComponent<Damageable>();
    }

    // Start is called before the first frame update
    void Start()
    {
        healthPercentage = playerDamageable.Health / playerDamageable.MaxHealth;
        healthText.text = playerDamageable.Health.ToString();
    }

    private void OnEnable()
    {
        playerDamageable.healthChanged.AddListener(OnPlayerHealthChanged);
    }

    private void OnDisable()
    {
        playerDamageable.healthChanged.RemoveListener(OnPlayerHealthChanged);
    }

    private void OnPlayerHealthChanged(int newHealth, int maxHealth)
    {
        healthPercentage = newHealth / maxHealth;
        healthText.text = newHealth.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
