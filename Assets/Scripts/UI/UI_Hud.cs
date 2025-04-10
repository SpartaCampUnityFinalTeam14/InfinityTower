using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Hud : UI
{
    [SerializeField] private TextMeshProUGUI floorText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI monsterCountText;

    [SerializeField] private Image costBar;

    [SerializeField] private FloatEventChannel OnCostChanged;
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
        OnCostChanged.UnregisterListener(SetCostBar);
        OnFloorCountChanged.UnregisterListener(SetFloorText);
        OnWaveCountChanged.UnregisterListener(SetWaveText);
        OnMonsterCountChanged.UnregisterListener(SetMonsterCountText);
    }

    void Subscribe()
    {
        OnCostChanged.RegisterListener(SetCostBar);
        OnFloorCountChanged.RegisterListener(SetFloorText);
        OnWaveCountChanged.RegisterListener(SetWaveText);
        OnMonsterCountChanged.RegisterListener(SetMonsterCountText);
    }

    void SetCostBar(float ratio)
    {
        costBar.fillAmount = ratio;
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
