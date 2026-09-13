public class EnemyHealth : Health
{
    private WaveManager waveManager;

    public void GetEnemySpawn(WaveManager newWaveManager)
    {
        if (newWaveManager != null)
            waveManager = newWaveManager;
    }

    protected override void Died()
    {
        if (ValidateReference(waveManager, nameof(waveManager))) waveManager.EnemyKilled(gameObject); 
        base.Died();
    }
}