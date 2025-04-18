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

    private void Awake()
    {
        UnregisterListeners();
        RegisterListeners();

        Init();
    }

    void Init()
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
                if (slot.data.id == index) slot.SetSelectedMark(true);
            }
        }

        ResetClickedMark();
    }

    void UnregisterListeners()
    {
        OnTowerSlotSelected.UnregisterListener(TowerSlotSelected);
    }

    void RegisterListeners()
    {
        OnTowerSlotSelected.RegisterListener(TowerSlotSelected);
    }

    public void ResetClickedMark()
    {
        foreach (var slot in slots)
        {
            slot.SetClickedMark(false);
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
                if (slot.data.id == index) slot.SetSelectedMark(true);
            }
        }

        ResetClickedMark();
    }

    public void Clear()
    {
        UnregisterListeners();
    }
}
