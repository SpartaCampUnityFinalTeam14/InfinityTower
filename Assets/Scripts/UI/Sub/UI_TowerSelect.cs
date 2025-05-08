using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_TowerSelect : MonoBehaviour
{
    [HideInInspector] public UI_Deck deck;

    [SerializeField] private GameObject scrollView;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    private List<UI_TowerSelectSlot> slots = new();

    public int slotWidth = 270;
    public int spacing = 20;
    public int poolCount = 7;
    private int prevIndex = -1;

    [SerializeField] private IntEventChannel OnTowerSlotSelected;
    [SerializeField] private IntEventChannel OnTowerLevelChanged;
    [SerializeField] private IntEventChannel OnTowerExpChanged;

    private void Awake()
    {
        scrollRect.onValueChanged.AddListener(UpdateSlots);

        UnregisterListeners();
        RegisterListeners();

        Init();
    }

    void UnregisterListeners()
    {
        OnTowerSlotSelected.UnregisterListener(TowerSlotSelected);
        OnTowerLevelChanged.UnregisterListener(UpdateTowerSlotLevel);
        OnTowerExpChanged.UnregisterListener(UpdateTowerSlotExp);
    }

    void RegisterListeners()
    {
        OnTowerSlotSelected.RegisterListener(TowerSlotSelected);
        OnTowerLevelChanged.RegisterListener(UpdateTowerSlotLevel);
        OnTowerExpChanged.RegisterListener(UpdateTowerSlotExp);
    }

    void Init()
    {
        foreach (Transform child in content) Destroy(child.gameObject);
        slots.Clear();

        InitContentSize();

        for (int i = 0; i < poolCount; i++)
        {
            UI_TowerSelectSlot slot = Util.InstantiatePrefabAndGetComponent<UI_TowerSelectSlot>(path: "UI/Sub/UI_TowerSelectSlot", parent: content);
            slots.Add(slot);
            slot.gameObject.SetActive(false);
        }

        UpdateSlots();
    }

    void InitContentSize()
    {
        int slotSize = slotWidth + spacing;
        int contentSize = DataManager.Instance.towerDict.Count * slotSize;
        content.sizeDelta = new Vector2(contentSize, content.sizeDelta.y);
    }

    void UpdateSlots(Vector2 scroll)
    {
        UpdateSlots();
    }

    public void UpdateSlots()
    {
        float scrollX = content.anchoredPosition.x;
        int slotSize = slotWidth + spacing;
        int curIndex = (int)MathF.Floor(-scrollX / slotSize);

        if (prevIndex == curIndex) return;
        prevIndex = curIndex;
        Debug.Log(curIndex);

        for(int i = 0; i < poolCount; i++)
        {
            int dataIndex = curIndex + i;
            if(0 <= dataIndex && dataIndex < DataManager.Instance.towerDict.Count)
            {
                var slot = slots[i];
                int dictKey = DataManager.Instance.towerDict.Keys.ToList()[dataIndex];
                slot.Init(dictKey);

                slot.gameObject.SetActive(true);
                RectTransform slotRectTransform = slot.GetComponent<RectTransform>();
                float slotX = dataIndex * slotSize;
                slotRectTransform.anchoredPosition = new Vector2(slotX, 0);
            }
            else slots[i].gameObject.SetActive(false);
        }
    }

    void TowerSlotSelected(int selectedIndex)
    {//selectedIndex 받아오기는 하는데, 어차피 SaveManager에 저장돼 있으니 가져와서 쓰고 맒
        foreach (var slot in slots)
        {
            slot.SetSelectedMark(false);
        }

        foreach (int index in SaveManager.Instance.playerData.selectedTowerIndex)
        {
            foreach (var slot in slots)
            {
                if (slot.id == index) slot.SetSelectedMark(true);
            }
        }
    }

    void UpdateTowerSlotLevel(int towerID)
    {
        foreach(var slot in slots)
        {
            if (slot.id == towerID)
            {
                slot.UpdateLevel();
                break;
            }
        }
    }

    void UpdateTowerSlotExp(int towerID)
    {
        foreach (var slot in slots)
        {
            if (slot.id == towerID)
            {
                slot.UpdateExp();
                break;
            }
        }
    }

    public void Clear()
    {
        UnregisterListeners();
    }
}
