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
    }

    public void SetSelectedMark(bool flag)
    {
        selectedMark.SetActive(flag);
    }
}
