using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterBase : Poolable
{
    private Floor floor;
    protected MonsterData data;

    List<Vector3> pathPoints;
    int curTileIdx = 0;

    public virtual void Init(int id, List<Vector3> path, Transform startPos, Floor floor)
    {
        this.floor = floor;

        data = new(DataManager.Instance.monsterDict[id]);//깊은 복사
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
                transform.position = Vector3.MoveTowards(transform.position, target, GetStat(StatType.Speed) * Time.deltaTime);
                yield return null;
            }

            transform.position = target;
            curTileIdx++;
            yield return null;
        }

        StageManager.Instance.TakeDamage((int)GetStat(StatType.Attack));
        Dead();
    }

    void Dead()
    {
        floor.SubrtactMonsterCount(1);
        PoolManager.Instance.Release(this);
    }
   
    public float GetStat(StatType type)
    {
        int iType = (int)type;
        var common = StageManager.Instance.abilityManager.commonMonsterAbilities;

        float origin = 0f;
        float abil = 0f;

        bool result = data.dictValue.TryGetValue(iType, out origin);
        abil = common.ContainsKey(iType) ? common[iType] : 0f;

        Debug.Assert(result, $"Not Find Type in DictionaryValue");
        
        return origin + abil;
    }
}
