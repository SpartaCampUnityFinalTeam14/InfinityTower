using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Hud : UI
{
    [SerializeField] private TextMeshProUGUI floorText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI monsterCountText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI hpText;

    [SerializeField] private Image costBar;

    [SerializeField] private FloatEventChannel OnCostChanged;
    [SerializeField] private IntEventChannel OnPlayerHPChanged;
    [SerializeField] private IntEventChannel OnFloorCountChanged;
    [SerializeField] private IntEventChannel OnWaveCountChanged;
    [SerializeField] private IntEventChannel OnMonsterCountChanged;

    protected override void Awake()
    {
        base.Awake();

        UnSubscribe();
        Subscribe();
    }

    void UnSubscribe()
    {
        OnCostChanged.UnregisterListener(SetCostText);
        OnPlayerHPChanged.UnregisterListener(SetHPText);
        OnFloorCountChanged.UnregisterListener(SetFloorText);
        OnWaveCountChanged.UnregisterListener(SetWaveText);
        OnMonsterCountChanged.UnregisterListener(SetMonsterCountText);
    }

    void Subscribe()
    {
        OnCostChanged.RegisterListener(SetCostText);
        OnPlayerHPChanged.RegisterListener(SetHPText);
        OnFloorCountChanged.RegisterListener(SetFloorText);
        OnWaveCountChanged.RegisterListener(SetWaveText);
        OnMonsterCountChanged.RegisterListener(SetMonsterCountText);
    }

    void SetCostText(float costAmount)
    {
        costText.text = $"코스트: {costAmount.ToString("N0")}";
    }

    void SetHPText(int hp)
    {
        hpText.text = $"체력: {hp.ToString("N0")}";
    }

    void SetFloorText(int floorCount)
    {
        floorText.text = floorCount.ToString();
    }

    void SetWaveText(int waveCount)
    {
        waveText.text = waveCount.ToString();
    }

    void SetMonsterCountText(int monsterCount)
    {
        monsterCountText.text = monsterCount.ToString();
    }

    public override void Clear()
    {
        UnSubscribe();
    }
}
