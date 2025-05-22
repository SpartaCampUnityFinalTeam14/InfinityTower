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
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image expBar;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private Button levelupButton;
    [SerializeField] private Button tempGetExpButton;

    [Header("우측 상단")]
    [SerializeField] private TextMeshProUGUI championHPText;
    [SerializeField] private TextMeshProUGUI championAtkText;

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
    [SerializeField] private Button closeButton;
    [SerializeField] private Button selectButton;

    [Header("이벤트채널")]
    [SerializeField] private IntEventChannel OnChampionSelected;
    [SerializeField] private IntEventChannel OnChampionLevelChanged;
    [SerializeField] private IntEventChannel OnChampionExpChanged;

    protected override void Awake()
    {
        base.Awake();

        backgroundButton.onClick.AddListener(() => UIManager.Instance.HideUI<UI_ChampionInfo>());
        closeButton.onClick.AddListener(() => UIManager.Instance.HideUI<UI_ChampionInfo>());
        selectButton.onClick.AddListener(SelectChampion);
        levelupButton.onClick.AddListener(Levelup);

        tempGetExpButton.onClick.AddListener(() => 
        {
            SetExp(++SaveManager.Instance.championLevelDict[data.id].exp);
            SaveManager.Instance.SaveChampionLevelData();
            OnChampionExpChanged.RaiseEvent(data.id);
        });

        skill1Button.onClick.AddListener(() => SetSkillInfo(data.skillid[0]));
        //skill2Button.onClick.AddListener(() => SetSkillInfo(data.skillID[1]));
    }

    public void Init(int id)
    {
        data = DataManager.Instance.championDict[id];

        SetChampion();
    }

    void SetChampion()
    {
        championIcon.sprite = Resources.Load<Sprite>($"Icons/Champion/Champion_{data.id}");
        championNameText.text = data.name;
        SetLevel(SaveManager.Instance.championLevelDict[data.id].level);
        SetExp(SaveManager.Instance.championLevelDict[data.id].exp);
        championHPText.text = data.hp.ToString();
        championAtkText.text = data.atk.ToString();

        SetSkillInfo(data.skillid[0]);
        skill1Icon.sprite = Resources.Load<Sprite>($"Icons/Skill/Skill_{data.skillid[0]}");
    }

    void Levelup()
    {
        int level = SaveManager.Instance.championLevelDict[data.id].level;
        if (level >= 10) return;

        int exp = SaveManager.Instance.championLevelDict[data.id].exp;
        int maxExp = DataManager.Instance.levelUpDict[level].requiredExp;

        if (exp >= maxExp)
        {
            level += 1;
            SaveManager.Instance.championLevelDict[data.id].level = level;
            SetLevel(level);

            exp -= maxExp;
            SaveManager.Instance.championLevelDict[data.id].exp = exp;
            SetExp(exp);
            
            SaveManager.Instance.SaveChampionLevelData();

            OnChampionLevelChanged.RaiseEvent(data.id);
            OnChampionExpChanged.RaiseEvent(data.id);
        }
    }

    void SetLevel(int level)
    {
        levelText.text = level.ToString();

        if (level >= 10) levelupButton.interactable = false;
    }

    void SetExp(int exp)
    {
        int level = SaveManager.Instance.championLevelDict[data.id].level;
        if (level >= 10)
        {
            expBar.fillAmount = 1f;
            expText.text = $"{exp} / Inf";
        }
        else
        {
            int maxExp = DataManager.Instance.levelUpDict[level].requiredExp;

            expBar.fillAmount = Mathf.Min(1f, (float)exp / maxExp);
            expText.text = $"{exp} / {maxExp}";
        }
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
