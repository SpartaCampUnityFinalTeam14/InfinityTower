using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI 연결")]
    public Image iconImage;
    public TMP_Text nameText;

    [Header("타워 설정")]
    private int towerID;
    private GameObject previewPrefab;
    private GameObject placedTowerPrefab;

    private GameObject previewObj;

    public void Init(int id)
    {
        towerID = id;

        // 이름 단순 표시 (원하면 Resources/타워 데이터로 확장 가능)
        nameText.text = $"타워 {towerID}";

        // 프리팹 로드 (예: Prefabs/TowerGhost_1, Prefabs/Tower_1)
        Sprite icon = Resources.Load<Sprite>($"Icons/Tower/Tower_{towerID}");
        if (icon)
            iconImage.sprite = icon;

        previewPrefab = Resources.Load<GameObject>($"Prefabs/Tower/TowerGhost_{towerID}");
        placedTowerPrefab = Resources.Load<GameObject>($"Prefabs/Tower/Tower_{towerID}");

        if (previewPrefab == null)
            Debug.LogError($"❌ previewPrefab 로드 실패: TowerGhost_{towerID}");

        if (placedTowerPrefab == null)
            Debug.LogError($"❌ placedTowerPrefab 로드 실패: Tower_{towerID}");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsCostEnough()) return;

        previewObj = Instantiate(previewPrefab);
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (previewObj == null)
            return;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        worldPos.z = 0;
        previewObj.transform.position = worldPos;

        Vector3Int cellPos = TilemapManager.Instance.tilemap.WorldToCell(worldPos);
        bool canPlace = TilemapManager.Instance.CanPlaceAt(cellPos);

        var renderers = previewObj.GetComponentsInChildren<SpriteRenderer>();
        foreach (var r in renderers)
        {
            r.color = canPlace ? new Color(0f, 1f, 0f, 0.8f) : new Color(1f, 0f, 0f, 0.8f); // 초록 or 빨강
        }   
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (TilemapManager.Instance == null || TilemapManager.Instance.tilemap == null)
        {
            Debug.LogError("TilemapManager or tilemap is NULL!");
            return;
        }
        
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector3Int cellPos = TilemapManager.Instance.tilemap.WorldToCell(worldPos);

        if (TilemapManager.Instance.CanPlaceAt(cellPos))
        {
            Vector3 spawnPos = TilemapManager.Instance.tilemap.CellToWorld(cellPos) +
                               TilemapManager.Instance.tilemap.cellSize / 2;

            if(IsCostEnough()) StageManager.Instance.UseCost(DataManager.Instance.towerDict[towerID].cost);

            //Instantiate(placedTowerPrefab, spawnPos, Quaternion.identity);
            
            var tower = PoolManager.Instance.Get(placedTowerPrefab).GetComponent<BaseTower>();
            tower.transform.position = spawnPos;
        }

        if (previewObj != null)
            Destroy(previewObj);
    }

    bool IsCostEnough()
    {
        int cost = DataManager.Instance.towerDict[towerID].cost;

        return StageManager.Instance.CheckCost(cost);
    }
}
