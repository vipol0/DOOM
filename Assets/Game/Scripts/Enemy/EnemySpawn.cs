using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private bool isDebug;
    
    public GameObject Spawn(GameObject prefab, WaveManager waveManager)
    {
        var enemy = Instantiate(prefab, transform.position, Quaternion.identity);
        enemy.GetComponent<EnemyHealth>()?.GetEnemySpawn(waveManager);
        enemy.GetComponent<EnemyBaseAttack>()?.GetCurrentWave(waveManager.CurrentWaveIndex);
        if (isDebug) Debug.Log($"[{gameObject.name}] Spawned {prefab.name}]");
        return enemy;
    }
}