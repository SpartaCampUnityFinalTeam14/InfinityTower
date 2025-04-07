using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    protected override void Awake()
    {
        isGlobal = false;

        base.Awake();
    }
}
