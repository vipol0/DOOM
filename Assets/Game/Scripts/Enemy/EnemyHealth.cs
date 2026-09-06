public class EnemyHealth : Health
{
    private EnemySpawn enemySpawn;

    public void GetEnemySpawn(EnemySpawn newEnemySpawn)
    {
        if (newEnemySpawn != null)
            enemySpawn = newEnemySpawn;
    }

    protected override void Died()
    {
        if (enemySpawn != null) enemySpawn.EnemyKilled(); 
        base.Died();
    }
}