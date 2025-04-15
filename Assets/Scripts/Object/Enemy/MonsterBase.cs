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
    private bool isDead;

    // 디버프 관련 상태 저장
    private Dictionary<BuffEffectType, Coroutine> debuffCoroutines = new Dictionary<BuffEffectType, Coroutine>();
    private float originalMoveSpeed;
    private float originalDefense;

    public virtual void Init(int id, List<Vector3> path, Transform startPos, Floor floor)
    {
        this.floor = floor;

        isDead = false;
        data = new(DataManager.Instance.monsterDict[id]);//깊은 복사
        currentHP = data.hp;
        transform.position = startPos.position;
        SetPath(path);

        //디버프 해제 후 원상복구를 위한 저장
        originalMoveSpeed = data.moveSpeed;
        originalDefense = defense/* = data.defense*/; //몬스터 데이터에 방어력 추가 시 주석 해제
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
        isDead = true;
        floor.SubrtactMonsterCount(1);
        PoolManager.Instance.Release(this);
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
        {
            return;
        }
        currentHP -= Mathf.RoundToInt(damage);
        if (currentHP <= 0)
        {
            Dead();
        }
    }

    //디버프 적용메서드
    public void ApplyDebuff(BuffEffectType type, float amount, float duration)
    {
        // 기존 디버프가 있으면 정지
        if (debuffCoroutines.TryGetValue(type, out Coroutine running))
        {
            StopCoroutine(running);
        }

        // 새로운 디버프 적용
        Coroutine routine = StartCoroutine(DebuffRoutine(type, amount, duration));
        debuffCoroutines[type] = routine;
    }

    private IEnumerator DebuffRoutine(BuffEffectType type, float amount, float duration)
    {
        switch (type)
        {
            case BuffEffectType.Slow:
                data.moveSpeed = Mathf.Max(0.1f, originalMoveSpeed - amount);
                break;

            case BuffEffectType.DefenseDown:
                defense = Mathf.Max(0, originalDefense - amount);
                break;
        }

        yield return new WaitForSeconds(duration);

        // 원래 값으로 복원
        switch (type)
        {
            case BuffEffectType.Slow:
                data.moveSpeed = originalMoveSpeed;
                break;

            case BuffEffectType.DefenseDown:
                defense = originalDefense;
                break;
        }

        // 디버프 딕셔너리에서 제거
        debuffCoroutines.Remove(type);
    }

}
