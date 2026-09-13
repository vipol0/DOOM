using UnityEngine;

public class EnemyBaseAttack : BaseMonoBehaviour
{
    [SerializeField] private float baseDamage = 20;
    [SerializeField] private float damageMultiplier = 2;
    [SerializeField] private int damageUpgradeInterval = 3;
    [SerializeField] private EnemyHealth enemyHealth;

    private float currentDamage;
    private int currentWave;

    private void Awake()
    {
        if (enemyHealth == null) enemyHealth = GetComponent<EnemyHealth>();

        ValidateReference(enemyHealth, nameof(enemyHealth));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || enemyHealth == null) return;

        other.GetComponent<PlayerHealth>().TakeDamage(currentDamage);
        enemyHealth.TakeDamage(1000);
    }

    public void GetCurrentWave(int wave)
    {
        currentWave = wave;

        if (currentWave < damageUpgradeInterval)
        {
            currentDamage = baseDamage;
            return;
        }

        int multiplierLevel = currentWave / damageUpgradeInterval;
        currentDamage = baseDamage * damageMultiplier * multiplierLevel;
    }
}