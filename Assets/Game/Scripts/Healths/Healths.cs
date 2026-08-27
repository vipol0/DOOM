using UnityEngine;
// using UnityEngine.Events;
using System;

public class Health : MonoBehaviour, IDamagable
{
    [Header("Healths Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private bool isDead = true;
#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private float currentHealthDebug;
#endif
    private float currentHealth;

    // [Header("Events")]
    // [SerializeField] private UnityEvent OnDeath;
    public event Action<float, float> OnHealthChanged;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

#if UNITY_EDITOR
    private void Update()
    {
        currentHealthDebug = currentHealth;
    }
#endif

    public void TakeDamage(float amount)
    {
        if (amount <= 0 || currentHealth <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            if (isDead) Died();
            
            // Die();
        }
    }
    
    public void Heal(float amount)
    {
        if (amount <= 0 || currentHealth <= 0 || currentHealth >= maxHealth) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Died()
    {
        Destroy(gameObject);
    }

    // private void Die()
    // {
    //     OnDeath?.Invoke();
    // }
}