using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public void LoadScene(string sceneName)
    {
        AnalyticsManager.SendEvent(sceneName, new Dictionary<string, object>
        {
            { "stage", "2-1" },
            { "result", "win" },
            { "score", 9876 }
        });

        var ui = UIManager.Instance.ShowUI<UI_Fade>();
        ui.FadeOut(() =>
        {
            PoolManager.Instance.Clear();
            UIManager.Instance.Clear();

            SceneManager.LoadScene(sceneName);
        });
    }
}
