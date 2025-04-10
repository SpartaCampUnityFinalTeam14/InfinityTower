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
    public int currentHP;
    //방어력 추가
    public float defense;

    private void Awake()
    {
        currentHP = data.hp;
    }

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

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            Dead();
        }
    }

    public void ApplyDebuff(DebuffType type, float amount, float duration)
    {
        StartCoroutine(DebuffCoroutine(type, amount, duration));
    }

    private IEnumerator DebuffCoroutine(DebuffType type, float amount, float duration)
    {
        switch (type)
        {
            case DebuffType.Slow:
                data.moveSpeed -= amount;
                break;

            case DebuffType.DefenseDown:
                defense -= amount;
                break;
        }

        yield return new WaitForSeconds(duration);

        // 디버프 종료 시 원래대로 복구
        switch (type)
        {
            case DebuffType.Slow:
                data.moveSpeed += amount;
                break;

            case DebuffType.DefenseDown:
                defense += amount;
                break;
        }
    }

}
