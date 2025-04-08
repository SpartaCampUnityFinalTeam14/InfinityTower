using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MonsterBase : Poolable
{
    protected MonsterData data;

    [SerializeField] float speed = 2f;
    List<Vector3> pathPoints;
    int curTileIdx = 0;

    public virtual void Init(int id, List<Vector3> path, Transform startPos)
    {
        data = DataManager.Instance.monsterDict[id];
        transform.position = startPos.position;
        SetPath(path);
    }

    public void SetPath(List<Vector3> path)
    {
        pathPoints = path;
        curTileIdx = 0;

        if (pathPoints != null && pathPoints.Count > 0)
        {
            transform.position = pathPoints[0]; // 시작 위치 설정
            StartCoroutine(MoveToPath());
        }
    }

    protected IEnumerator MoveToPath()
    {
        while (curTileIdx < pathPoints.Count)
        {
            Vector3 target = pathPoints[curTileIdx];

            while (Vector3.Distance(transform.position, target) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
                yield return null;
            }

            transform.position = target;
            curTileIdx++;
            yield return null;
        }

        StageManager.Instance.TakeDamage(data.damage);
        PoolManager.Instance.Release(this);
    }
}
