using UnityEngine;

public class Scene_Stage : Scene
{
    protected override void Init()
    {
        base.Init();
        UIManager.Instance.ShowUI<UI_Hud>();
    }
}
