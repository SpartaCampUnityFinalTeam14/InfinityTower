using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChampionSelect : MonoBehaviour
{
    [HideInInspector] public UI_Deck deck;

    [SerializeField] private GameObject scrollView;
    [SerializeField] private ScrollChild scrollRect;
    [SerializeField] private RectTransform content;
    private List<UI_ChampionSlot> slots = new();

    public int slotWidth = 270;
    public int spacing = 20;
    private int slotSize;
    private int visibleSlotCount;
    public int poolCount = 6;
    private int prevIndex = 0;
    private List<int> keys = new();

    [SerializeField] private IntEventChannel OnChampionSelected;
    [SerializeField] private IntEventChannel OnChampionLevelChanged;
    [SerializeField] private IntEventChannel OnChampionExpChanged;

    private void Awake()
    {
        scrollRect.onValueChanged.AddListener(UpdateSlots);

        UnregisterEvent();
        RegisterEvent();

        keys = SaveManager.Instance.championLevelDict.Keys.ToList();
        slotSize = slotWidth + spacing;
        visibleSlotCount = Mathf.CeilToInt(scrollRect.GetComponent<RectTransform>().rect.width / slotSize);

        Init();
    }

    void UnregisterEvent()
    {
        OnChampionSelected.UnregisterListener(SetSelectedChampion);
        OnChampionLevelChanged.UnregisterListener(UpdateChampionSlotLevel);
        OnChampionExpChanged.UnregisterListener(UpdateChampionSlotExp);
    }

    void RegisterEvent()
    {
        OnChampionSelected.RegisterListener(SetSelectedChampion);
        OnChampionLevelChanged.RegisterListener(UpdateChampionSlotLevel);
        OnChampionExpChanged.RegisterListener(UpdateChampionSlotExp);
    }

    public void Init()
    {
        foreach(Transform child in content) Destroy(child.gameObject);
        slots.Clear();

        InitContentSize();

        for (int i = 0; i < poolCount; i++)
        {
            UI_ChampionSlot slot = Util.InstantiatePrefabAndGetComponent<UI_ChampionSlot>(path: "UI/Sub/UI_ChampionSlot", parent: content);
            slots.Add(slot);
            slot.gameObject.SetActive(false);
        }

        InitSlots();
        SetSelectedChampion(SaveManager.Instance.playerData.selectedChampionIndex);
    }

    void InitContentSize()
    {
        int contentSize = SaveManager.Instance.championLevelDict.Count * slotSize;
        content.sizeDelta = new Vector2(contentSize, content.sizeDelta.y);
    }

    void InitSlots()
    {
        for (int i = 0; i < poolCount; i++)
        {
            int dataIndex = i;
            if (0 <= dataIndex && dataIndex < SaveManager.Instance.championLevelDict.Count)
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

    public void ResetScroll()
    {
        scrollRect.ResetScroll();
    }
    public void ResetAllSlot()
    {
        foreach (var slot in slots)
        {
            slot.UpdateSlot();
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
            for (int i = 0; i < delta; i++)
            {
                int targetIndex = curIndex + poolCount - delta + i;
                if (targetIndex < keys.Count)
                {
                    RecycleToRight(targetIndex);
                    Debug.Log($"오른쪽으로, {i}, {targetIndex}");
                }
            }
        }
        else
        {
            for (int i = 1; i <= -delta; i++)
            {
                int targetIndex = prevIndex - i;
                if (targetIndex >= 0)
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

    void SetSelectedChampion(int id)
    {
        SaveManager.Instance.playerData.selectedChampionIndex = id;

        foreach (UI_ChampionSlot slot in slots)
        {
            slot.SetSelectedMark(slot.id == id);
        }

        SaveManager.Instance.SavePlayerData();
    }

    void UpdateChampionSlotLevel(int championID)
    {
        foreach (var slot in slots)
        {
            if (slot.id == championID)
            {
                slot.UpdateLevel();
                break;
            }
        }
    }

    void UpdateChampionSlotExp(int championID)
    {
        foreach (var slot in slots)
        {
            if (slot.id == championID)
            {
                slot.UpdateExp();
                break;
            }
        }
    }

    public void Clear()
    {
        UnregisterEvent();
    }
}
