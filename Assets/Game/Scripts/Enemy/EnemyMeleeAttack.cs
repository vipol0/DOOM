using System.Collections;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float startAttackCooldown = 0.2f;
    [SerializeField] private int damage = 10;

    private PlayerHealth playerHealth;
    private Coroutine cooldownReload;
    private bool canAttack;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth == null) return;

        RestartCooldown(startAttackCooldown);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerHealth = null;
        canAttack = false;

        if (cooldownReload != null)
        {
            StopCoroutine(cooldownReload);
            cooldownReload = null;
        }
    }

    private void Update()
    {
        if (!canAttack || playerHealth == null) return;

        Attack();
    }

    private void Attack()
    {
        canAttack = false;

        playerHealth.TakeDamage(damage);

        RestartCooldown(attackCooldown);
    }

    private void RestartCooldown(float cooldown)
    {
        if (cooldownReload != null) StopCoroutine(cooldownReload);

        cooldownReload = StartCoroutine(CooldownReload(cooldown));
    }

    private IEnumerator CooldownReload(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);

        canAttack = true;
        cooldownReload = null;
    }
}