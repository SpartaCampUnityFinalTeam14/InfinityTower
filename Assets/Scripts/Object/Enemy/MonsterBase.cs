using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MonsterBase : Poolable
{
    private Floor floor;
    protected MonsterData data;

    List<Vector3> pathPoints;
    int curTileIdx = 0;

    List<AbilityData> abilities;
    Dictionary<int, float> baseStat = new();
    Dictionary<int, float> abilityStat = new();

    public int ID => data.id;

    public virtual void Init(int id, List<Vector3> path, Transform startPos, Floor floor)
    {
        this.floor = floor;

        data = new(DataManager.Instance.monsterDict[id]);//깊은 복사
        
        StageManager.Instance.abilityManager.OnAbilityChanged += UpdateAbility;
        UpdateAbility();

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
                transform.position = Vector3.MoveTowards(transform.position, target, data.moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = target;
            curTileIdx++;
            yield return null;
        }

        StageManager.Instance.TakeDamage(data.damage);
        Dead();
    }

    void Dead()
    {
        floor.SubrtactMonsterCount(1);
        PoolManager.Instance.Release(this);
    }

    void UpdateAbility()
    {
        abilities = StageManager.Instance.abilityManager.GetAbilities(data);

        foreach (var ability in abilities)
        {
            for (int i = 0; i < ability.valueType.Count; i++)
            {
                if (abilityStat.ContainsKey(ability.valueType[i]))
                {
                    abilityStat[ability.valueType[i]] += ability.value[i];
                }
                else
                {
                    abilityStat.Add(ability.valueType[i], ability.value[i]);
                }
            }
        }
    }

    public float GetStat(StatType type)
    {
        int iType = (int)type;

        float origin = baseStat.ContainsKey(iType) ? baseStat[iType] : 0f;
        float abil = abilityStat.ContainsKey(iType) ? abilityStat[iType] : 0f;

        return origin + abil;
    }
}
