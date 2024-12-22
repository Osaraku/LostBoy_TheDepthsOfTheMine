using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthIndicator : MonoBehaviour
{
    public TMP_Text healthText;
    Damageable playerDamageable;
    public float healthPercentage;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerDamageable = player.GetComponent<Damageable>();
    }

    // Start is called before the first frame update
    void Start()
    {
        healthPercentage = calculateHealthPercentage(playerDamageable.Health, playerDamageable.MaxHealth);
        healthText.text = playerDamageable.Health.ToString();
    }

    public float calculateHealthPercentage(float currentHealth, float maxHealth)
    {
        return (float)currentHealth / maxHealth;
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
        healthPercentage = calculateHealthPercentage(newHealth, maxHealth);

        if (newHealth < 0)
        {
            healthText.text = "0";
        }
        else
        {
            healthText.text = newHealth.ToString();
        }

        if (healthPercentage > 0.7)
        {
            transform.GetChild(3).gameObject.SetActive(true);
        }
        else if (healthPercentage > 0.45 && healthPercentage <= 0.7)
        {
            transform.GetChild(3).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(true);
        }
        else if (healthPercentage > 0 && healthPercentage <= 0.45)
        {
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else if (healthPercentage <= 0)
        {
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(0).gameObject.SetActive(true);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
