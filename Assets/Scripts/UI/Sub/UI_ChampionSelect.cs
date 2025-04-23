using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChampionSelect : MonoBehaviour
{
    [HideInInspector] public UI_Deck deck;

    [SerializeField] private GameObject scrollView;
    [SerializeField] private Transform slotParent;
    private List<UI_ChampionSlot> slots = new();

    [SerializeField] private IntEventChannel OnChampionSelected;
    [SerializeField] private IntEventChannel OnChampionLevelChanged;
    [SerializeField] private IntEventChannel OnChampionExpChanged;

    private void Awake()
    {
        UnregisterEvent();
        RegisterEvent();

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

    void Init()
    {
        foreach(Transform child in slotParent) Destroy(child.gameObject);
        slots.Clear();

        foreach(var data in DataManager.Instance.championDict)
        {
            UI_ChampionSlot slot = Util.InstantiatePrefabAndGetComponent<UI_ChampionSlot>(path: "UI/Sub/UI_ChampionSlot", parent: slotParent);
            slots.Add(slot);

            slot.Init(data.Value.id);
        }

        SetSelectedChampion(SaveManager.Instance.playerData.selectedChampionIndex);
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
