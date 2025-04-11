using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TowerSelect : UI
{
    [SerializeField] private Button closeButton;

    [SerializeField] private Transform slotParent;
    private List<UI_TowerSelectSlot> slots = new();

    [SerializeField] private List<Image> selectedTowerSlots = new(5);

    protected override void Awake()
    {
        base.Awake();

        closeButton.onClick.AddListener(() => UIManager.Instance.HideUI<UI_TowerSelect>());

        Init();
    }

    void Init()
    {
        foreach (Transform child in slotParent) Destroy(child.gameObject);
        slots.Clear();

        for (int i = 0; i < DataManager.Instance.championDict.Count; i++)
        {
            UI_TowerSelectSlot slot = Util.InstantiatePrefabAndGetComponent<UI_TowerSelectSlot>(path: "UI/Sub/UI_TowerSelectSlot", parent: slotParent);
            slots.Add(slot);

            slot.Init(DataManager.Instance.championDict[i].id);
        }
    }

    public override void Clear()
    {

    }
}
