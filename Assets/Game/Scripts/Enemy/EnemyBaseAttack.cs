using System;
using UnityEngine;

public class EnemyBaseAttack : BaseMonoBehaviour
{
    [SerializeField] private int damage = 20;
    [SerializeField] private EnemyHealth enemyHealth;

    private void Awake()
    {
        if (enemyHealth == null) gameObject.GetComponent<EnemyHealth>();
        ValidateReference(enemyHealth, nameof(enemyHealth));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || enemyHealth == null) return;
        other.GetComponent<PlayerHealth>().TakeDamage(damage);
        enemyHealth.TakeDamage(1000);
    }
}
