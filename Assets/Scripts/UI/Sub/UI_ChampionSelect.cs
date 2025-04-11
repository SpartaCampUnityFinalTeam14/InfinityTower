using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChampionSelect : MonoBehaviour
{
    [SerializeField] private Image championImage;

    [SerializeField] private GameObject scrollView;
    [SerializeField] private Transform slotParent;
    private List<UI_ChampionSlot> slots = new();

    [SerializeField] private IntEventChannel OnChampionSelected;

    private void Awake()
    {
        UnregisterEvent();
        RegisterEvent();

        Init();
    }

    void UnregisterEvent()
    {
        OnChampionSelected.UnregisterListener(SetSelectedChampion);
    }

    void RegisterEvent()
    {
        OnChampionSelected.RegisterListener(SetSelectedChampion);
    }

    void Init()
    {
        foreach(Transform child in slotParent) Destroy(child.gameObject);
        slots.Clear();

        int selectedId = SaveManager.Instance.playerData.selectedChampionIndex;
        for(int i = 0; i < DataManager.Instance.championDict.Count; i++)
        {
            UI_ChampionSlot slot = Util.InstantiatePrefabAndGetComponent<UI_ChampionSlot>(path: "UI/Sub/UI_ChampionSlot", parent: slotParent);
            slots.Add(slot);

            slot.Init(DataManager.Instance.championDict[i].id);
        }

        SetSelectedChampion(selectedId);
    }

    void SetSelectedChampion(int id)
    {
        SaveManager.Instance.playerData.selectedChampionIndex = id;

        foreach (UI_ChampionSlot slot in slots)
        {
            slot.SetSelectedMark(slot.id == id);
        }
    }

    public void Clear()
    {
        UnregisterEvent();
    }
}
