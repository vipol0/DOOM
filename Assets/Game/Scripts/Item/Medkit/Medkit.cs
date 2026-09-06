using UnityEngine;

public class Medkit : Item
{
    [SerializeField] private int healHealth = 20;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var health = other.GetComponent<PlayerHealth>();

        if (health == null) return;

        if (health.CurrentHealth >= health.MaxHealth) return;

        health.Heal(healHealth);
        OnGetItem();
        Destroy(gameObject);
    }
}