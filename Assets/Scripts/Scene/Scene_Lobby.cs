using UnityEngine;

public class Scene_Lobby : Scene
{
    protected override void Init()
    {
        base.Init();

        Time.timeScale = 1f;
        UIManager.Instance.HideUI<UI_LobbyTutorial>();
        UIManager.Instance.ShowUI<UI_Main>();
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlayBGM(BGM.LobbyBGM);
    }
}
