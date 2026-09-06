using System;
using UnityEngine;

public abstract class Health : MonoBehaviour, IDamagable
{
    [Header("Healths Settings")] [SerializeField]
    private float maxHealth = 100f;
    [SerializeField] private bool isDead = true;
    
#if UNITY_EDITOR
    [Header("Debug")] [SerializeField] private float currentHealthDebug;
#endif
    public event Action<float, float> OnHealthChanged;
    public float CurrentHealth { get; private set; }

    public float MaxHealth => maxHealth;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

#if UNITY_EDITOR
    private void Update()
    {
        currentHealthDebug = CurrentHealth;
    }
#endif

    public void TakeDamage(float amount)
    {
        if (amount <= 0 || CurrentHealth <= 0) return;

        CurrentHealth -= amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, maxHealth);

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth <= 0 && isDead) Died();
    }

    public void Heal(float amount)
    {
        if (amount <= 0 || CurrentHealth <= 0 || CurrentHealth >= maxHealth) return;

        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, maxHealth);

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    protected virtual void Died()
    {
        Destroy(gameObject);
    }
}