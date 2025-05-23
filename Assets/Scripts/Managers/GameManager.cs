using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    protected override void Awake()
    {
        base.Awake();

        AnalyticsManager.SendEvent("Funnel_Step", new Dictionary<string, object>        
        {
            { "Funnel_Step_Number", 1 }
        });
    }
    
    public void LoadScene(string sceneName)
    {
        var ui = UIManager.Instance.ShowUI<UI_Fade>();
        ui.FadeOut(() =>
        {
            PoolManager.Instance.Clear();
            UIManager.Instance.Clear();

            SceneManager.LoadScene(sceneName);

            ui.Hide();
        });
    }
}
