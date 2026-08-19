using TMPro;
using UnityEngine;

public class HealthsText : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private TextMeshProUGUI text;

    private void Awake()
    {
        if (text == null) text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        health.OnHealthChanged += UpdateText;
    }

    private void OnDisable()
    {
        health.OnHealthChanged -= UpdateText;
    }

    private void UpdateText(float currentHealth, float maxHealth)
    {
        if (text == null)
        {
            Debug.LogWarning($"[{name}] has no {nameof(Health)} set");
            return;
        }

        text.text = "Healths: " + currentHealth + "/" + maxHealth;
    }
}