using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Hud : UI
{
    [SerializeField] private Image damageImage;
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;
    private GameObject damageGO;
    private Sequence damageSequence;

    [SerializeField] private RectTransform floorInfoBackgroundRect;
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

        damageGO = damageImage.gameObject;
        damageGO.SetActive(false);
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
        if (hp < int.Parse(hpText.text.Split("/")[0]))
        {
            hpText.color = Color.red;
            hpText.DOColor(Color.white, 0.3f).SetUpdate(true);
            hpText.rectTransform.localScale = Vector3.one * 1.3f;
            hpText.rectTransform.DOScale(Vector3.one, 0.3f).SetUpdate(true);
        }
        hpText.text = $"{hp.ToString("N0")}/{StageManager.Instance.GetMaxHP()}";
    }

    public void TakeDamage()
    {
        damageGO.SetActive(true);
        damageImage.color = startColor;

        damageSequence?.Kill();

        damageSequence = DOTween.Sequence().SetUpdate(true);
        damageSequence
            .Join(damageImage
                .DOColor(endColor, 0.3f)
                .OnComplete(() => damageGO.SetActive(false)))
            .Join(Camera.main.DOShakePosition(0.2f, 0.3f));
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

        LayoutRebuilder.ForceRebuildLayoutImmediate(floorInfoBackgroundRect);
    }

    public override void Clear()
    {
        UnSubscribe();
    }

    private void OnDestroy()
    {
        UnSubscribe();
    }
}
