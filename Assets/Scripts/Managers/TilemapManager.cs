using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    public static TilemapManager Instance;

    public Tilemap tilemap; // 설치 타일맵
    private HashSet<Vector3Int> occupiedCells = new(); // 설치된 셀 정보
    
    private GameObject cellIndicatorPrefab;
    private List<GameObject> cellIndicators = new();

    private void Awake()
    {
        Instance = this;
        
        cellIndicatorPrefab = Resources.Load<GameObject>("Prefabs/Floors/TileCell");

        if (cellIndicatorPrefab == null)
            Debug.LogError("❌ CellIndicator 프리팹을 Resources/Prefabs 에서 찾을 수 없습니다.");
    }

    /// 이 셀이 타워 설치 가능한지 여부
    public bool CanPlaceAt(Vector3Int cellPos)
    {
        TileBase tile = tilemap.GetTile(cellPos);

        if (tile == null)
        {
            //Debug.Log(cellPos + " , 타일 없음");
            return false;
        }
        if (occupiedCells.Contains(cellPos)) 
        {
            //Debug.Log(cellPos + " , 타일 이미 꽉 차 있음");
            return false; 
        }
        //Debug.Log(cellPos + " , 타일 있음");
        return true;
    }

    /// 이 셀에 타워를 설치함 → 타워 설치 후 호출
    public void RegisterOccupiedCell(Vector3Int cellPos)
    {
        if (!occupiedCells.Contains(cellPos))
            occupiedCells.Add(cellPos);
    }

    /// 이 셀을 비움 (예: 타워 파괴 시)
    public void UnregisterOccupiedCell(Vector3Int cellPos)
    {
        if (occupiedCells.Contains(cellPos))
            occupiedCells.Remove(cellPos);
    }

    public List<Vector3Int> GetAllPlaceableCells()
    {
        List<Vector3Int> result = new();
        BoundsInt bounds = tilemap.cellBounds;

        foreach (var pos in bounds.allPositionsWithin)
        {
            if (!tilemap.HasTile(pos)) continue;
            if (CanPlaceAt(pos)) result.Add(pos);
        }

        Debug.Log($"📦 설치 가능한 셀 개수: {result.Count}");
        return result;
    }
    
    public void ShowAllPlaceableCells()
    {
        ClearIndicators(); // 먼저 초기화
        BoundsInt bounds = tilemap.cellBounds;
        StageManager.Instance.timeScaleManager.PushTimeScale(0.2f);

        foreach (var pos in bounds.allPositionsWithin)
        {
            if (!tilemap.HasTile(pos)) continue;

            bool canPlace = CanPlaceAt(pos);

            Vector3 worldPos = tilemap.CellToWorld(pos) + tilemap.cellSize / 2f;
            GameObject indicator = Instantiate(cellIndicatorPrefab, worldPos, Quaternion.identity);
            indicator.transform.localScale = tilemap.cellSize * 0.9f;


            var renderer = indicator.GetComponent<SpriteRenderer>();
            renderer.color = canPlace ? new Color(0f, 1f, 0f, 0.3f) : new Color(1f, 0f, 0f, 0.3f);

            cellIndicators.Add(indicator);
        }
    }

    public void ClearIndicators()
    {
        foreach (var obj in cellIndicators)
        {
            if (obj != null) Destroy(obj);
        }
        cellIndicators.Clear();
        StageManager.Instance.timeScaleManager.PopTimeScale();
    }
}