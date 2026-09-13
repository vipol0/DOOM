using UnityEngine.SceneManagement;

public class PlayerHealth : Health
{
    protected override void Died()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}