using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathManager : Singleton<PathManager>
{
    #region A*Node
    class AStarNode
    {
        public Vector3Int position;
        public AStarNode parent;
        public float gCost; // 현재까지 이동 비용
        public float hCost; // 목표까지 예상 비용 (휴리스틱)
        public float F => gCost + hCost;

        public AStarNode(Vector3Int pos, AStarNode parent, float g, float h)
        {
            position = pos;
            this.parent = parent;
            gCost = g;
            hCost = h;
        }
    }
    #endregion

    [SerializeField] Tilemap pathTilemap;
    [SerializeField] Transform startPos;
    [SerializeField] Transform endPos;

    Vector3Int startTile;
    Vector3Int endTile;

    public List<Vector3> pathPoints = new List<Vector3>();

    private readonly Vector3Int[] directions = {
        Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right
    };

    protected override void Awake()
    {
        isGlobal = false;

        base.Awake();
    }

    public void Init(Tilemap path, Transform start, Transform end)
    {
        pathTilemap = path;
        startPos = start;
        endPos = end;
        FindPath();
    }

    void FindPath()
    {
        startTile = pathTilemap.WorldToCell(startPos.position);
        endTile = pathTilemap.WorldToCell(endPos.position);
        PathFind();
    }

    #region A*
    void PathFind()
    {
        pathPoints.Clear();

        List<AStarNode> open = new List<AStarNode>();
        HashSet<Vector3Int> close = new HashSet<Vector3Int>();

        AStarNode startNode = new AStarNode(startTile, null, 0, GetHeuristic(startTile, endTile));
        open.Add(startNode);

        while (open.Count > 0)
        {
            open.Sort((a, b) => a.F.CompareTo(b.F));
            AStarNode current = open[0];
            open.RemoveAt(0);

            if (current.position == endTile)
            {
                RetracePath(current);
                return;
            }

            close.Add(current.position);

            foreach (Vector3Int dir in directions)
            {
                Vector3Int neighborPos = current.position + dir;

                if (!pathTilemap.HasTile(neighborPos) || close.Contains(neighborPos))
                    continue;

                float newG = current.gCost + 1;
                
                AStarNode existing = open.Find(n => n.position == neighborPos);
                if (existing == null)
                {
                    // 새로운 노드
                    float h = GetHeuristic(neighborPos, endTile);
                    open.Add(new AStarNode(neighborPos, current, newG, h));
                }
                else if (newG < existing.gCost)
                {
                    // 비용이 적은 경로로 갱신
                    existing.gCost = newG;
                    existing.parent = current;
                }
            }
        }

        Debug.LogAssertion("경로 탐색 실패");
    }

    void RetracePath(AStarNode endNode)
    {
        AStarNode current = endNode;
        while (current != null)
        {
            Vector3 worldPos = pathTilemap.CellToWorld(current.position) + pathTilemap.cellSize / 2;
            pathPoints.Insert(0, worldPos);
            current = current.parent;
        }
    }

    float GetHeuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // 맨해튼 거리
    }
    #endregion

    #region DFS
    //public void Init(Tilemap _pathTilemap)
    //{
    //    pathTilemap = _pathTilemap;

    //    PathFind();
    //}

    //void PathFind()
    //{
    //    pathPoints.Clear();
    //    visited.Clear();

    //    if (!DFS(startPoint))
    //    {
    //        Debug.LogAssertion("��� Ž�� ����");
    //    }
    //    else
    //    {
    //        Debug.LogAssertion($"��� Ž�� �Ϸ�");
    //    }
    //}

    //bool DFS(Vector3Int current)
    //{
    //    if (!pathTilemap.HasTile(current) || visited.Contains(current))
    //        return false;

    //    visited.Add(current);
    //    pathPoints.Add(pathTilemap.CellToWorld(current) + pathTilemap.cellSize / 2);

    //    if (current == endPoint && pathPoints.Count > 1)
    //        return true;

    //    foreach (Vector3Int dir in directions)
    //    {
    //        Vector3Int next = current + dir;
    //        if (DFS(next))
    //            return true;
    //    }

    //    // ���ٸ� ���̸� �ǵ�����
    //    pathPoints.RemoveAt(pathPoints.Count - 1);
    //    return false;
    //}

    //public List<Vector3> GetPathPoints()
    //{
    //    return pathPoints;
    //}
    #endregion
}
