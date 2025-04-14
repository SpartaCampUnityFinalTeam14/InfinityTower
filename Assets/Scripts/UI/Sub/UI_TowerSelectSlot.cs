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

    [SerializeField] private IntEventChannel OnTowerSelected;

    private void Awake()
    {
        selectButton.onClick.AddListener(SelectTower);
    }

    public void Init(int id)
    {
        data = DataManager.Instance.towerDict[id];

        nameText.text = data.name;
        //스프라이트 지정해줘야 함
    }

    void SelectTower()
    {
        OnTowerSelected.RaiseEvent(data.id);
        SetClickedMark(true);
    }

    public void SetSelectedMark(bool flag)
    {
        selectedMark.SetActive(flag);
    }

    public void SetClickedMark(bool flag)
    {
        clickedMark.SetActive(flag);
    }
}
