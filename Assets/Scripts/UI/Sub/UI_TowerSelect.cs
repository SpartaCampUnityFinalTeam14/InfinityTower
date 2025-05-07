using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TowerSelect : MonoBehaviour
{
    [HideInInspector] public UI_Deck deck;

    [SerializeField] private GameObject scrollView;
    [SerializeField] private Transform slotParent;
    private List<UI_TowerSelectSlot> slots = new();

    [SerializeField] private IntEventChannel OnTowerSlotSelected;
    [SerializeField] private IntEventChannel OnTowerLevelChanged;
    [SerializeField] private IntEventChannel OnTowerExpChanged;

    private void Awake()
    {
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

    public void Init()
    {
        foreach (Transform child in slotParent) Destroy(child.gameObject);
        slots.Clear();

        foreach(var data in DataManager.Instance.towerDict)
        {
            UI_TowerSelectSlot slot = Util.InstantiatePrefabAndGetComponent<UI_TowerSelectSlot>(path: "UI/Sub/UI_TowerSelectSlot", parent: slotParent);
            slots.Add(slot);

            slot.Init(data.Value.id);
        }

        foreach(int index in SaveManager.Instance.playerData.selectedTowerIndex)
        {
            foreach(var slot in slots)
            {
                if (slot.id == index) slot.SetSelectedMark(true);
            }
        }
    }

    public void UpdateSlots()
    {
        foreach(var slot in slots)
        {
            slot.UpdateSlot();
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
