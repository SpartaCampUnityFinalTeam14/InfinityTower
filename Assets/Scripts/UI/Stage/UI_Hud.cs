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

    [SerializeField] private Button speedButton;
    [SerializeField] private TextMeshProUGUI speedText; 
    [SerializeField] private Button pauseButton;

    [SerializeField] private FloatEventChannel OnCostChanged;
    [SerializeField] private IntEventChannel OnPlayerHPChanged;
    [SerializeField] private IntEventChannel OnFloorCountChanged;
    [SerializeField] private IntEventChannel OnWaveCountChanged;
    [SerializeField] private IntEventChannel OnMonsterCountChanged;

    private bool isSpeed;

    protected override void Awake()
    {
        base.Awake();

        speedButton.onClick.AddListener(ToggleSpeed);
        pauseButton.onClick.AddListener(Pause);

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

    void ToggleSpeed()
    {
        isSpeed = !isSpeed;

        if (isSpeed)
        {
#if UNITY_EDITOR
            StageManager.Instance.timeScaleManager.SetBaseTimeScale(5f);
            speedText.text = "X5";
#else
            StageManager.Instance.timeScaleManager.SetBaseTimeScale(2f);
            speedText.text = "X2";
#endif
        }
        else
        {
            StageManager.Instance.timeScaleManager.SetBaseTimeScale(1f);
            speedText.text = "X1";
        }
    }

    void Pause()
    {
        UIManager.Instance.GetUI<UIPause>().TogglePause();
    }

    void SetCostText(float costAmount)
    {
        costText.text = costAmount.ToString("N0");
    }

    void SetHPText(int hp)
    {
        hpText.text = $"{hp.ToString("N0")}/{StageManager.Instance.GetMaxHP()}";
    }

    void SetFloorText(int floorCount)
    {
        floorText.text = $"{floorCount.ToString()}/{StageManager.Instance.GetMaxFloor()}";
    }

    void SetWaveText(int waveCount)
    {
        waveText.text = $"{waveCount.ToString()}/{StageManager.Instance.GetMaxWaveCount()}";
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
