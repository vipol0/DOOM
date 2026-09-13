using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthsBar : BaseMonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Image image;

    private void Awake()
    {
        ValidateReference(health, nameof(health));
        
        if (!ValidateReference(image, nameof(image)))
        {
            image =  GetComponent<Image>();
        }
    }

    private void OnEnable()
    {
        if (health == null) return;
        health.OnHealthChanged += UpdateImage;
    }

    private void OnDisable()
    {
        if (health == null) return;
        health.OnHealthChanged -= UpdateImage;
    }

    private void UpdateImage(float currentHealth, float maxHealth)
    {
        if (image == null) return;
        
        image.fillAmount = currentHealth / maxHealth;
    }
}
