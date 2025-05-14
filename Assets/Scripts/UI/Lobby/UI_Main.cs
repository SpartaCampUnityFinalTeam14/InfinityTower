using UnityEngine;

public class UI_Main : UI
{
    [SerializeField] private NestedScrollManager scrollManager;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Clear()
    {
        scrollManager.UnregisterListeners();

        base.Clear();
    }
}
