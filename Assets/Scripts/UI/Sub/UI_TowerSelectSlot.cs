using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_TowerSelectSlot : MonoBehaviour
{
    public int id;
    private TowerData data;

    [SerializeField] private Button selectButton;
    [SerializeField] private Image towerImage;
    [SerializeField] private GameObject selectedMark;
    [SerializeField] private GameObject clickedMark;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image expBar;
    [SerializeField] private TextMeshProUGUI expText;

    [SerializeField] private IntEventChannel OnTowerSelected;

    private void Awake()
    {
        selectButton.onClick.AddListener(() => UIManager.Instance.ShowStackUI<UI_TowerInfo>().Init(data.id));
    }

    public void Init(int id)
    {
        this.id = id;
        data = DataManager.Instance.towerDict[id];

        //스프라이트 지정해줘야 함
        nameText.text = data.name;
        descriptionText.text = data.description;

        SetLevel(SaveManager.Instance.towerLevelDict[id].level);
        SetExp(SaveManager.Instance.towerLevelDict[id].exp);
    }

    public void SetSelectedMark(bool flag)
    {
        selectedMark.SetActive(flag);
    }

    public void SetClickedMark(bool flag)
    {
        clickedMark.SetActive(flag);
    }

    public void UpdateLevel()
    {
        SetLevel(SaveManager.Instance.towerLevelDict[id].level);
    }

    void SetLevel(int level)
    {
        levelText.text = level.ToString();
    }

    public void UpdateExp()
    {
        SetExp(SaveManager.Instance.towerLevelDict[id].exp);
    }

    void SetExp(int exp)
    {
        int level = SaveManager.Instance.towerLevelDict[id].level;
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
