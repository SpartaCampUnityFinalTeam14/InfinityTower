using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChampionSlot : MonoBehaviour
{
    public int id;
    private ChampionData data;

    [SerializeField] private Button selectButton;
    [SerializeField] private Image championImage;
    [SerializeField] private GameObject selectedMark;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image expBar;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private GameObject ownedMark;

    private void Awake()
    {
        selectButton.onClick.AddListener(() => UIManager.Instance.ShowStackUI<UI_ChampionInfo>().Init(id));
        SetSelectedMark(false);
    }

    public void Init(int id)
    {
        this.id = id;
        data = DataManager.Instance.championDict[id];

        UpdateSlot();
    }

    public void UpdateSlot()
    {
        nameText.text = data.name;
        descriptionText.text = data.description;
        //아이디에 맞춰서 스프라이트 찾아와야 함

        SetLevel(SaveManager.Instance.championLevelDict[id].level);
        SetExp(SaveManager.Instance.championLevelDict[id].exp);
        SetOwnedMark(SaveManager.Instance.championLevelDict[id].level == 0);
    }

    public void SetSelectedMark(bool flag)
    {
        selectedMark.SetActive(flag);
    }

    public void SetOwnedMark(bool flag)
    {
        ownedMark.SetActive(flag);
        selectButton.interactable = !flag;
    }

    public void UpdateLevel()
    {
        SetLevel(SaveManager.Instance.championLevelDict[data.id].level);
    }

    void SetLevel(int level)
    {
        levelText.text = level.ToString();
    }

    public void UpdateExp()
    {
        SetExp(SaveManager.Instance.championLevelDict[data.id].exp);
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
}
