using UnityEngine;
using System.Collections.Generic;

public class TowerListUI : MonoBehaviour
{
    public GameObject towerSlotPrefab;
    public Transform contentParent;
    [SerializeField] private EventChannel OnFloorStarted;

    List<TowerSlotUI> slots = new();

    private void Awake()
    {
        OnFloorStarted.RegisterListener(ResetSlots);
    }

    void Start()
    {
        Init();
    }

    void Init()
    {
        foreach(Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        List<int> ownedTowerIDs = StageManager.Instance.selectedTowers;
        Debug.Log($"🧪 플레이어가 가진 타워 개수: {ownedTowerIDs.Count}");

        foreach (int id in ownedTowerIDs)
        {
            if (id < 0) continue;

            GameObject slot = Instantiate(towerSlotPrefab, contentParent);

            TowerSlotUI slotUI = slot.GetComponent<TowerSlotUI>();
            slots.Add(slotUI);
            if (slotUI != null)
            {
                slotUI.Init(id);
            }
        }
    }

    public void ResetSlots()
    {
        List<int> ownedTowerIDs = StageManager.Instance.selectedTowers;

        for(int i = 0; i < slots.Count; i++)
        {
            slots[i].Init(ownedTowerIDs[i]);
        }
    }

    private void OnDestroy()
    {
        OnFloorStarted.UnregisterListener(ResetSlots);
    }
}