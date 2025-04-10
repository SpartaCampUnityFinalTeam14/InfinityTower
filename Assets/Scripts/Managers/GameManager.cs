using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public void LoadScene(string sceneName)
    {
        PoolManager.Instance.Clear();
        UIManager.Instance.Clear();

        SceneManager.LoadScene(sceneName);
    }
}
