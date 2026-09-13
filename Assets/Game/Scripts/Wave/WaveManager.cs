using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    [Serializable]
    private struct Wave
    {
        public float waveTime;
        public int prefabCount;
        public GameObject[] prefabs;
    }

    [SerializeField] private EnemySpawn[] spawns;
    [SerializeField] private List<Wave> waves;

    [Header("Spawn Settings")] [SerializeField]
    private float spawnCooldown = 2f;

    [SerializeField] private float minSpawnCooldown = 0.2f;
    [SerializeField] private int cooldownDecreaseEveryNWaves = 3;
    [SerializeField] private float cooldownDecreaseAmount = 0.2f;

    [Header("Wave Settings")] [SerializeField]
    private float breakTime = 5f;

    [Header("Debug")] [SerializeField] private bool isDebug;

    public int CurrentWaveIndex { get; set; } = -1;

    private int enemiesAlive;
    private bool isWaveActive;

    private Coroutine waveCoroutine;
    private Coroutine spawnCoroutine;
    private Coroutine breakCoroutine;

    private readonly List<GameObject> activeEnemies = new();

    public event Action<int> OnWaveStarted;
    public event Action<float> OnWaveTimeChanged;
    public event Action OnBreakStarted;
    public event Action<float> OnBreakTimeChanged;

    private void Start()
    {
        StartNextWave();
    }

    public void StartNextWave()
    {
        CurrentWaveIndex++;

        if (CurrentWaveIndex >= waves.Count)
        {
            if (isDebug) Debug.Log($"[{gameObject.name}] All waves completed!");

            return;
        }

        activeEnemies.Clear();
        isWaveActive = true;

        var currentSpawnCooldown = GetCurrentSpawnCooldown();

        OnWaveStarted?.Invoke(CurrentWaveIndex + 1);

        waveCoroutine = StartCoroutine(WaveRoutine());

        if (isDebug)
            Debug.Log($"[{gameObject.name}] " + $"Wave {CurrentWaveIndex + 1} started." +
                      $"Spawn cooldown: {currentSpawnCooldown:F2} sec.");
    }

    private IEnumerator WaveRoutine()
    {
        var currentWave = waves[CurrentWaveIndex];

        enemiesAlive = currentWave.prefabCount;

        spawnCoroutine = StartCoroutine(
            SpawnEnemies(currentWave)
        );

        yield return StartCoroutine(
            WaveTimer(currentWave.waveTime)
        );

        DestroyRemainingEnemies();

        EndWave();
    }

    private IEnumerator SpawnEnemies(Wave wave)
    {
        var currentSpawnCooldown = GetCurrentSpawnCooldown();

        for (var i = 0; i < wave.prefabCount; i++)
        {
            if (!isWaveActive) yield break;

            if (spawns != null && spawns.Length > 0 && wave.prefabs != null && wave.prefabs.Length > 0)
            {
                var randomSpawn = GetRandomSpawnPoint();

                var randomPrefab = wave.prefabs[Random.Range(0, wave.prefabs.Length)];

                if (randomSpawn != null && randomPrefab != null)
                {
                    var spawnedEnemy = randomSpawn.Spawn(randomPrefab, this);

                    if (spawnedEnemy != null) activeEnemies.Add(spawnedEnemy);
                }
            }

            yield return new WaitForSeconds(currentSpawnCooldown);
        }

        spawnCoroutine = null;
    }

    private IEnumerator WaveTimer(float waveTime)
    {
        if (waveTime <= 0f) yield break;

        var remainingTime = waveTime;

        OnWaveTimeChanged?.Invoke(1f);

        while (remainingTime > 0f && isWaveActive)
        {
            var progress = remainingTime / waveTime;

            OnWaveTimeChanged?.Invoke(progress);

            remainingTime -= Time.deltaTime;

            yield return null;
        }

        OnWaveTimeChanged?.Invoke(0f);
    }

    private float GetCurrentSpawnCooldown()
    {
        if (cooldownDecreaseEveryNWaves <= 0) return spawnCooldown;

        var decreaseSteps = CurrentWaveIndex / cooldownDecreaseEveryNWaves;

        var currentCooldown = spawnCooldown - decreaseSteps * cooldownDecreaseAmount;

        return Mathf.Max(minSpawnCooldown, currentCooldown);
    }

    private EnemySpawn GetRandomSpawnPoint()
    {
        if (spawns == null || spawns.Length == 0) return null;

        var randomIndex =
            Random.Range(0, spawns.Length);

        return spawns[randomIndex];
    }

    private void DestroyRemainingEnemies()
    {
        for (var i = activeEnemies.Count - 1; i >= 0; i--)
            if (activeEnemies[i] != null)
                Destroy(activeEnemies[i]);

        activeEnemies.Clear();
    }

    private void EndWave()
    {
        if (!isWaveActive) return;

        isWaveActive = false;

        if (waveCoroutine != null)
        {
            StopCoroutine(waveCoroutine);
            waveCoroutine = null;
        }

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        if (isDebug) Debug.Log($"[{gameObject.name}] " + $"Wave {CurrentWaveIndex + 1} completed.");

        if (CurrentWaveIndex + 1 < waves.Count)
        {
            breakCoroutine = StartCoroutine(BreakRoutine());
        }
        else
        {
            if (isDebug) Debug.Log($"[{gameObject.name}] " + "This was the final wave.");
        }
    }

    private IEnumerator BreakRoutine()
    {
        if (isDebug) Debug.Log($"[{gameObject.name}] " + "Break between waves: " + $"{breakTime} sec.");

        OnBreakStarted?.Invoke();

        var remainingTime = breakTime;

        while (remainingTime > 0f)
        {
            var progress = remainingTime / breakTime;

            OnBreakTimeChanged?.Invoke(progress);

            remainingTime -= Time.deltaTime;

            yield return null;
        }

        OnBreakTimeChanged?.Invoke(0f);

        breakCoroutine = null;

        StartNextWave();
    }

    public void EnemyKilled(GameObject enemy)
    {
        if (enemy == null) return;

        if (!activeEnemies.Contains(enemy)) return;

        activeEnemies.Remove(enemy);

        enemiesAlive--;

        if (enemiesAlive <= 0 && isWaveActive) EndWave();
    }
}