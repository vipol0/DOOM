using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnCooldown = 15;
    [SerializeField] private int enemyCountSpawn = 15;

    [Header("Debug")] [SerializeField] private bool isDebug = true;

    private int currentWave;
    private int enemiesAlive;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            currentWave++;

            var enemyCount = enemyCountSpawn + currentWave * 3;

            if (isDebug) Debug.Log("Start wave: " + currentWave);

            for (var i = 0; i < enemyCount; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(0.5f);
            }

            yield return new WaitUntil(() => enemiesAlive <= 0);

            Debug.Log($"Wave {currentWave} is end");
            
            yield return new WaitForSeconds(spawnCooldown);
        }
    }

    private void SpawnEnemy()
    {
        var spawnPoint =
            spawnPoints[Random.Range(0, spawnPoints.Length)];

        var enemy = Instantiate(
            enemyPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        enemy.GetComponent<EnemyHealth>()?.GetEnemySpawn(this);

        enemiesAlive++;
    }

    public void EnemyKilled()
    {
        enemiesAlive--;
    }
}