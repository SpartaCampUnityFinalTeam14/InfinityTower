using UnityEngine;

public class Scene_Lobby : Scene
{
    protected override void Init()
    {
        base.Init();

        UIManager.Instance.ShowUI<UI_Lobby>();
    }
}
