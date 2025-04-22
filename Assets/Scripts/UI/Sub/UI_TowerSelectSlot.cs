using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_TowerSelectSlot : MonoBehaviour
{
    [HideInInspector] public TowerData data;

    [SerializeField] private Button selectButton;
    [SerializeField] private Image towerImage;
    [SerializeField] private GameObject selectedMark;
    [SerializeField] private GameObject clickedMark;
    [SerializeField] private TextMeshProUGUI nameText;

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
        data = DataManager.Instance.towerDict[id];

        nameText.text = data.name;
        //스프라이트 지정해줘야 함

        //레벨 및 경험치 세팅해줘야 함
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

    void SetLevel(int level)
    {
        levelText.text = level.ToString();
    }

    void SetExp(int exp)
    {
        expText.text = exp.ToString();
        //최대 경험치도 체크해야 함
    }
}
