using TMPro;
using UnityEngine;

public class HealthsText : BaseText
{
    [SerializeField] private Health health;
    [SerializeField] private TextMeshProUGUI text;

    private void Awake()
    {
        if (text == null) text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        if (health == null) return;
        health.OnHealthChanged += UpdateText;
    }

    private void OnDisable()
    {
        if (health == null) return;
        health.OnHealthChanged -= UpdateText;
    }

    private void UpdateText(float currentHealth, float maxHealth)
    {
        if (text == null) return;

        text.SetText($"{currentHealth}");
    }
}