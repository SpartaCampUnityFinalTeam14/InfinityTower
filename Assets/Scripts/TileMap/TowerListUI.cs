using UnityEngine;
using System.Collections.Generic;

public class TowerListUI : MonoBehaviour
{
    public GameObject towerSlotPrefab;
    public Transform contentParent;

    void Start()
    {
        Init();
    }

    void Init()
    {
        List<int> ownedTowerIDs = SaveManager.Instance.playerData.selectedTowerIndex;
        Debug.Log($"🧪 플레이어가 가진 타워 개수: {ownedTowerIDs.Count}");

        foreach (int id in ownedTowerIDs)
        {
            GameObject slot = Instantiate(towerSlotPrefab, contentParent);

            TowerSlotUI slotUI = slot.GetComponent<TowerSlotUI>();
            if (slotUI != null)
            {
                slotUI.Init(id);
            }
        }
    }
}