using TMPro;
using UnityEngine;

public class UI_Main : UI
{
    [SerializeField] private NestedScrollManager scrollManager;
    [SerializeField] private TextMeshProUGUI goldText;

    [SerializeField] private EventChannel OnGoldChanged;

    protected override void Awake()
    {
        base.Awake();

        UnregisterListeners();
        RegisterListeners();

        UpdateGold();
    }

    void UnregisterListeners()
    {
        OnGoldChanged.UnregisterListener(UpdateGold);
    }

    void RegisterListeners()
    {
        OnGoldChanged.RegisterListener(UpdateGold);
    }

    void UpdateGold()
    {
        int gold = SaveManager.Instance.playerData.gold;
        goldText.text = string.Format("{0:N0}", gold);
    }

    public override void Clear()
    {
        UnregisterListeners(); ;
        scrollManager.UnregisterListeners();

        base.Clear();
    }
}
