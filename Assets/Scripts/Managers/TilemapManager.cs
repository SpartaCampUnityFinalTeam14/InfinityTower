using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    public static TilemapManager Instance;

    public Tilemap tilemap; // 여기에 's' 타일맵을 연결해놓은 상태

    private void Awake()
    {
        Instance = this;
    }

    public bool CanPlaceAt(Vector3Int cellPos)
    {
        TileBase tile = tilemap.GetTile(cellPos);

        if (tile != null)
        {
            return true; // 배치 가능
        }
        return false;
    }
    
    public List<Vector3Int> GetAllPlaceableCells()
    {
        List<Vector3Int> result = new();

        BoundsInt bounds = tilemap.cellBounds;

        foreach (var pos in bounds.allPositionsWithin)
        {
            if (!tilemap.HasTile(pos)) continue;
            if (CanPlaceAt(pos))
            {
                result.Add(pos);
            }
        }
        Debug.Log($"📦 설치 가능한 셀 개수: {result.Count}");
        return result;
    }
}