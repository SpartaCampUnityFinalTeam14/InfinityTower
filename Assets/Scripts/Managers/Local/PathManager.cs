using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathManager : MonoBehaviour
{
    public Tilemap pathTilemap;

    public Vector3Int startPoint;
    public Vector3Int endPoint;

    private List<Vector3> pathPoints = new List<Vector3>();
    private HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

    private readonly Vector3Int[] directions = {
        Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right
    };

    public void Init(Tilemap _pathTilemap)
    {
        pathTilemap = _pathTilemap;

        PathFind();
    }

    void PathFind()
    {
        pathPoints.Clear();
        visited.Clear();

        if (!DFS(startPoint))
        {
            Debug.LogAssertion("경로 탐색 실패");
        }
        else
        {
            Debug.LogAssertion($"경로 탐색 완료");
        }
    }

    bool DFS(Vector3Int current)
    {
        if (!pathTilemap.HasTile(current) || visited.Contains(current))
            return false;

        visited.Add(current);
        pathPoints.Add(pathTilemap.CellToWorld(current) + pathTilemap.cellSize / 2);

        // 경로 시작 위치와 끝 위치가 같거나 붙어있는 경우 예외 처리
        if (current == endPoint && pathPoints.Count > 1)
            return true;

        foreach (Vector3Int dir in directions)
        {
            Vector3Int next = current + dir;
            if (DFS(next))
                return true;
        }

        // 막다른 길이면 되돌리기
        pathPoints.RemoveAt(pathPoints.Count - 1);
        return false;
    }

    public List<Vector3> GetPathPoints()
    {
        return pathPoints;
    }
}
