using System.Collections.Generic;
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
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image expBar;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private Button levelupButton;
    [SerializeField] private Button tempGetExpButton;

    [Header("우측 상단")]
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("우측 하단")]
    [SerializeField] private Transform statInfoBackgroundTransform;
    private List<UI_StatEach> stats = new();

    [Header("하단")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button selectButton;

    [Header("이벤트채널")]
    [SerializeField] private IntEventChannel OnTowerSelected;
    [SerializeField] private IntEventChannel OnTowerLevelChanged;
    [SerializeField] private IntEventChannel OnTowerExpChanged;

    protected override void Awake()
    {
        base.Awake();

        backgroundButton.onClick.AddListener(() => UIManager.Instance.HideUI<UI_TowerInfo>());
        closeButton.onClick.AddListener(() => UIManager.Instance.HideUI<UI_TowerInfo>());
        selectButton.onClick.AddListener(SelectTower);
        levelupButton.onClick.AddListener(Levelup);

        tempGetExpButton.onClick.AddListener(() => 
        {
            SetExp(++SaveManager.Instance.towerLevelDict[data.id].exp);
            SaveManager.Instance.SaveTowerLevelData();
            OnTowerExpChanged.RaiseEvent(data.id);
        });
    }

    public void Init(int id)
    {
        data = DataManager.Instance.towerDict[id];

        SetTower();
    }

    void SetTower()
    {
        towerIcon.sprite = Resources.Load<Sprite>($"Icons/Tower/Tower_{data.id}");
        towerNameText.text = data.name;
        levelupButton.interactable = true;
        SetLevel(SaveManager.Instance.towerLevelDict[data.id].level);
        SetExp(SaveManager.Instance.towerLevelDict[data.id].exp);

        foreach(Transform child in statInfoBackgroundTransform)
        {
            Destroy(child.gameObject);
        }

        foreach(int id in data.statType)
        {
            UI_StatEach stat = Util.InstantiatePrefabAndGetComponent<UI_StatEach>(path: "UI/Sub/UI_StatEach", parent: statInfoBackgroundTransform);
            stat.Init((StatType)id, data.GetStatName(id), data.GetStatValue(id));
            stats.Add(stat);
        }
    }

    void Levelup()
    {
        int level = SaveManager.Instance.towerLevelDict[data.id].level;
        if (level >= 10) return;

        int exp = SaveManager.Instance.towerLevelDict[data.id].exp;
        int maxExp = DataManager.Instance.levelUpDict[level].requiredExp;

        if (exp >= maxExp)
        {
            level += 1;
            SaveManager.Instance.towerLevelDict[data.id].level = level;
            SetLevel(level);

            exp -= maxExp;
            SaveManager.Instance.towerLevelDict[data.id].exp = exp;
            SetExp(exp);
            
            SaveManager.Instance.SaveTowerLevelData();

            OnTowerLevelChanged.RaiseEvent(data.id);
            OnTowerExpChanged.RaiseEvent(data.id);
        }
    }

    void SetLevel(int level)
    {
        levelText.text = level.ToString();

        if (level >= 10) levelupButton.interactable = false;
    }

    void SetExp(int exp)
    {
        int level = SaveManager.Instance.towerLevelDict[data.id].level;
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
