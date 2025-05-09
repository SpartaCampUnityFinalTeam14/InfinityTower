using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public void LoadScene(string sceneName)
    {
        var ui = UIManager.Instance.ShowUI<UI_Fade>();
        ui.FadeOut(() =>
        {
            PoolManager.Instance.Clear();
            UIManager.Instance.Clear();

            SceneManager.LoadScene(sceneName);
        });
    }
}
