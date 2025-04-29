using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_TowerInfo : UI
{
    private TowerData data;

    [Header("배경")]
    [SerializeField] private Button backgroundButton;

    [Header("좌측")]
    [SerializeField] private TextMeshProUGUI towerNameText;
    [SerializeField] private Image towerIcon;
    [SerializeField] private Image expBar;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private Button levelupButton;

    [Header("우측 상단")]
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("우측 하단")]
    [SerializeField] private TextMeshProUGUI towerCostText;
    [SerializeField] private TextMeshProUGUI towerDamageText;
    [SerializeField] private TextMeshProUGUI towerAttackSpeedText;
    [SerializeField] private TextMeshProUGUI towerRangeText;

    [Header("하단")]
    [SerializeField] private Button selectButton;
    [SerializeField] private Button closeButton;

    [Header("이벤트채널")]
    [SerializeField] private IntEventChannel OnTowerSelected;

    protected override void Awake()
    {
        base.Awake();

        backgroundButton.onClick.AddListener(() => UIManager.Instance.HideUI<UI_TowerInfo>());
        closeButton.onClick.AddListener(() => UIManager.Instance.HideUI<UI_TowerInfo>());
        selectButton.onClick.AddListener(SelectTower);
        levelupButton.onClick.AddListener(Levelup);
    }

    public void Init(int id)
    {
        data = DataManager.Instance.towerDict[id];

        SetTower();
    }

    void SetTower()
    {
        towerNameText.text = data.name;
        //exp 세팅
        //스프라이트 세팅
        towerCostText.text = data.GetStatValue(StatType.Cost).ToString();
        towerDamageText.text = data.GetStatValue(StatType.Damage).ToString();
        towerAttackSpeedText.text = data.GetStatValue(StatType.ActiveSpeed).ToString();
        towerRangeText.text = data.GetStatValue(StatType.Range).ToString();
    }

    void Levelup()
    {
        Debug.Log("레벨업 구현해야 함");
    }

    void SelectTower()
    {
        OnTowerSelected.RaiseEvent(data.id);
        UIManager.Instance.HideUI<UI_TowerInfo>();
    }

    public override void Clear()
    {
        base.Clear();
    }
}
