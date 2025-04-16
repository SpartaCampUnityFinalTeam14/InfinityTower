using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChampionInfo : UI
{
    private ChampionData data;

    [Header("배경")]
    [SerializeField] private Button backgroundButton;

    [Header("좌측")]
    [SerializeField] private TextMeshProUGUI championNameText;
    [SerializeField] private Image championIcon;
    [SerializeField] private Image expBar;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private Button levelupButton;

    [Header("우측 상단")]
    [SerializeField] private TextMeshProUGUI championHPText;

    [Header("우측 하단")]
    [SerializeField] private Image skill1Icon;
    [SerializeField] private Button skill1Button;
    [SerializeField] private Image skill2Icon;
    [SerializeField] private Button skill2Button;
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillInfoText;
    [SerializeField] private TextMeshProUGUI skillDamageText;
    [SerializeField] private TextMeshProUGUI skillCoolTimeText;

    [Header("하단")]
    [SerializeField] private Button selectButton;
    [SerializeField] private Button closeButton;

    [Header("이벤트채널")]
    [SerializeField] private IntEventChannel OnChampionSelected;

    protected override void Awake()
    {
        base.Awake();

        backgroundButton.onClick.AddListener(() => UIManager.Instance.HideUI<UI_ChampionInfo>());
        closeButton.onClick.AddListener(() => UIManager.Instance.HideUI<UI_ChampionInfo>());
        selectButton.onClick.AddListener(SelectChampion);

        skill1Button.onClick.AddListener(() => SetSkillInfo(data.skillId[0]));
        skill2Button.onClick.AddListener(() => SetSkillInfo(data.skillId[1]));
    }

    public void Init(int id)
    {
        data = DataManager.Instance.championDict[id];

        SetChampion();
    }

    void SetChampion()
    {
        championNameText.text = data.name;
        //스프라이트 세팅해야 함
        //EXP 세팅해야 함
        championHPText.text = data.hp.ToString();

        SetSkillInfo(data.skillId[0]);
    }

    void Levelup()
    {

    }

    void SetSkillInfo(int id)
    {
        SkillData skill = DataManager.Instance.skillDict[id];
        skillNameText.text = skill.name;
        skillInfoText.text = skill.description;
        skillDamageText.text = skill.multiplier.ToString();
        skillCoolTimeText.text = skill.coolTime.ToString();
    }

    void SelectChampion()
    {
        OnChampionSelected.RaiseEvent(data.id);
        UIManager.Instance.HideUI<UI_ChampionInfo>();
    }

    public override void Clear()
    {
        base.Clear();
    }
}
