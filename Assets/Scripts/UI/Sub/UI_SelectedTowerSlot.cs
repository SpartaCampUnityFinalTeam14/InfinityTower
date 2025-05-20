using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectedTowerSlot : MonoBehaviour
{
    [HideInInspector] public int slotId = 0;
    [HideInInspector] public int towerId = -1;
    [HideInInspector] public UI_Deck deck;

    [SerializeField] private RectTransform towerSlotTransform;
    [SerializeField] private Button towerSlotButton;
    [SerializeField] private Image towerIcon;
    [SerializeField] private TextMeshProUGUI towerNameText;

    [SerializeField] private IntEventChannel OnTowerSlotSelected;

    private void Awake()
    {
        towerSlotButton.onClick.AddListener(OpenInfoUI);
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

        towerIcon.sprite = Resources.Load<Sprite>($"Icons/Tower/Tower_{index}");
        towerId = index;
        towerNameText.text = DataManager.Instance.towerDict[index].name;

        //RotateSlotRandom();
    }

    void ClearSlot()
    {
        towerId = -1;
        towerIcon.sprite = null;
        towerNameText.text = null;
    }

    void RotateSlotRandom()
    {
        float randomRotZ = Random.Range(-5f, 5f);
        Vector3 curRot = towerSlotTransform.eulerAngles;
        towerSlotTransform.eulerAngles = new Vector3(curRot.x, curRot.y, randomRotZ);
    }

    void OpenInfoUI()
    {
        OnTowerSlotSelected.RaiseEvent(slotId);
    }
}
