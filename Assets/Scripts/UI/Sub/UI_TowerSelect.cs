using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TowerSelect : MonoBehaviour
{
    [SerializeField] private GameObject scrollView;
    [SerializeField] private Transform slotParent;
    private List<UI_TowerSelectSlot> slots = new();

    [SerializeField] private List<Image> selectedTowerSlots = new(5);

    private void Awake()
    {
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
}
