using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_TowerInfo : UI
{
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
    }

    public override void Clear()
    {
        base.Clear();
    }
}
