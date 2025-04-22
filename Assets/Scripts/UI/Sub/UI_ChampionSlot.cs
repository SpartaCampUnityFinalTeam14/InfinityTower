using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChampionSlot : MonoBehaviour
{
    public int id;
    ChampionData data;

    [SerializeField] private Button selectButton;
    [SerializeField] private Image championImage;
    [SerializeField] private GameObject selectedMark;
    [SerializeField] private TextMeshProUGUI nameText;

    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image expBar;
    [SerializeField] private TextMeshProUGUI expText;

    private void Awake()
    {
        selectButton.onClick.AddListener(() => UIManager.Instance.ShowStackUI<UI_ChampionInfo>().Init(id));
        SetSelectedMark(false);
    }

    public void Init(int id)
    {
        this.id = id;
        data = DataManager.Instance.championDict[id];

        nameText.text = data.name;
        //아이디에 맞춰서 스프라이트 찾아와야 함

        //레벨 및 경험치 세팅해야 함
        SetLevel(SaveManager.Instance.championLevelDict[id].level);
        SetExp(SaveManager.Instance.championLevelDict[id].exp);
    }

    public void SetSelectedMark(bool flag)
    {
        selectedMark.SetActive(flag);
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
