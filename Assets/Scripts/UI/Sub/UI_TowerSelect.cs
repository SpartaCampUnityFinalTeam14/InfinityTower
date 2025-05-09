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
    private int slotSize;
    private int visibleSlotCount;
    //private int poolSize;
    public int poolCount = 6;
    private int prevIndex = 0;
    private List<int> keys = new();

    [SerializeField] private IntEventChannel OnTowerSlotSelected;
    [SerializeField] private IntEventChannel OnTowerLevelChanged;
    [SerializeField] private IntEventChannel OnTowerExpChanged;

    private void Awake()
    {
        scrollRect.onValueChanged.AddListener(UpdateSlots);

        UnregisterListeners();
        RegisterListeners();

        keys = DataManager.Instance.towerDict.Keys.ToList();
        slotSize = slotWidth + spacing;
        visibleSlotCount = Mathf.CeilToInt(scrollRect.GetComponent<RectTransform>().rect.width / slotSize);
        //poolSize = poolCount * slotSize;

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

        InitSlots();
    }

    void InitContentSize()
    {
        int contentSize = DataManager.Instance.towerDict.Count * slotSize;
        content.sizeDelta = new Vector2(contentSize, content.sizeDelta.y);
    }

    void InitSlots()
    {
        for (int i = 0; i < poolCount; i++)
        {
            int dataIndex = i;
            if (0 <= dataIndex && dataIndex < DataManager.Instance.towerDict.Count)
            {
                var slot = slots[i];
                slot.Init(keys[dataIndex]);

                slot.gameObject.SetActive(true);
                RectTransform slotRectTransform = slot.GetComponent<RectTransform>();
                float slotX = dataIndex * (slotSize);
                slotRectTransform.anchoredPosition = new Vector2(slotX, 0);
            }
            else slots[i].gameObject.SetActive(false);
        }
    }

    void UpdateSlots(Vector2 scroll)
    {
        UpdateSlots();
    }

    public void UpdateSlots()
    {
        float scrollX = -content.anchoredPosition.x;
        int curIndex = Mathf.FloorToInt(scrollX / slotSize);

        curIndex = Mathf.Clamp(curIndex, 0, Mathf.Max(0, keys.Count - visibleSlotCount - 1));

        int delta = curIndex - prevIndex;

        if (delta == 0) return;

        Debug.Log(curIndex + ", " + delta);
        if (delta > 0)
        {
            for(int i = 0; i < delta; i++)
            {
                int targetIndex = curIndex + poolCount - delta + i;
                if(targetIndex < keys.Count)
                {
                    RecycleToRight(targetIndex);
                    Debug.Log($"오른쪽으로, {i}, {targetIndex}");
                }
            }
        }
        else
        {
            for(int i = 1; i <= -delta; i++)
            {
                int targetIndex = prevIndex - i;
                if(targetIndex >= 0)
                {
                    RecycleToLeft(targetIndex);
                    Debug.Log($"왼쪽으로, {i}, {targetIndex}");
                }
            }
        }

        prevIndex = curIndex;
    }

    void RecycleToLeft(int dataIndex)
    {
        var rightSlot = slots.Last();
        slots.RemoveAt(slots.Count - 1);
        slots.Insert(0, rightSlot);
        rightSlot.Init(keys[dataIndex]);

        RectTransform rect = rightSlot.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(dataIndex * slotSize, rect.anchoredPosition.y);
    }

    void RecycleToRight(int dataIndex)
    {
        var leftSlot = slots[0];
        slots.RemoveAt(0);
        slots.Add(leftSlot);
        leftSlot.Init(keys[dataIndex]);

        RectTransform rect = leftSlot.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(dataIndex * slotSize, rect.anchoredPosition.y);
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
