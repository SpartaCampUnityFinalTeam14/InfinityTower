using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectedTowerSlot : MonoBehaviour
{
    [HideInInspector] public int slotId = 0;
    [HideInInspector] public int towerId = -1;
    [HideInInspector] public UI_Deck deck;

    [SerializeField] private Button towerSlotButton;
    [SerializeField] private Image towerIcon;
    [SerializeField] private TextMeshProUGUI towerNameText;

    [SerializeField] private IntEventChannel OnTowerSlotSelected;

    private void Awake()
    {
        towerSlotButton.onClick.AddListener(() => OnTowerSlotSelected.RaiseEvent(slotId));
    }

    public void Init(int slotId, int towerId, UI_Deck deck)
    {
        this.slotId = slotId;
        this.towerId = towerId;
        this.deck = deck;
    }

    public void SetSelectedTower(int index)
    {
        if (index < 0)
        {
            ClearSlot();
            return;
        }

        //스프라이트 설정해야 함
        towerId = index;
        towerNameText.text = DataManager.Instance.towerDict[index].name;
    }

    void ClearSlot()
    {
        towerId = -1;
        towerIcon.sprite = null;
        towerNameText.text = null;
    }
}
