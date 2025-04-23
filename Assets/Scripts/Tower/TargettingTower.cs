using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;



public abstract class TargettingTower : BaseTower
{
    protected List<GameObject> targets;
    //범위 안에 들어온 적 리스트
    List<MonsterBase> enemiesInRange;
    //범위 안에 있는 아군 타워 리스트
    List<TargettingTower> towerInRange;

    protected override void Update()
    {
        base.Update();
    }

    public virtual void FindTargets()
    {
        // targetingRule, targetType, targetCount 등을 이용해 타겟팅
        targets = new List<GameObject>();
        enemiesInRange = new List<MonsterBase>();
        towerInRange = new List<TargettingTower>();

        //범위 내 유닛 탐색
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, towerData.GetStatValue(TowerStatType.Range));        

        //범위 안에 있는 적과 아군 판별
        foreach (Collider2D hit in hits)
        {
            switch (towerData.TargetType)
            {
                case TargetType.Enemy:
                    if (hit.CompareTag("Enemy"))
                    {
                        MonsterBase enemy = hit.GetComponent<MonsterBase>();
                        if (enemy != null)
                            enemiesInRange.Add(enemy);
                    }
                break;

                case TargetType.Tower:
                    if (hit.CompareTag("Tower"))
                    {
                        TargettingTower tower = hit.GetComponent<TargettingTower>();
                        if (tower != null && tower != this)
                            towerInRange.Add(tower);
                    }
                break;
            }

        }

        // 2. 타겟팅 룰 적용
        switch (towerData.TargetType)
        {
            //타겟이 적일 경우
            case TargetType.Enemy:
                switch (towerData.TargettingRule)
                {
                    case TargettingRule.Nearest: // 타워와 가장 가까운 적
                        enemiesInRange = enemiesInRange
                            .OrderBy(m => Vector2.Distance(transform.position, m.transform.position))
                            .ToList();
                        break;

                    case TargettingRule.Farthest: // 타워와 가장 먼 적
                        enemiesInRange = enemiesInRange
                            .OrderByDescending(m => Vector2.Distance(transform.position, m.transform.position))
                            .ToList();
                        break;

                    case TargettingRule.LowestHP: // 체력이 가장 낮은 적
                        enemiesInRange = enemiesInRange
                            .OrderBy(m => m.currentHP)
                            .ToList();
                        break;

                    case TargettingRule.HighestHP: // 체력이 가장 높은 적
                        enemiesInRange = enemiesInRange
                            .OrderByDescending(m => m.currentHP)
                            .ToList();
                        break;

                    default:
                        break;
                }
                break;

            //타겟이 아군일 경우
            case TargetType.Tower:
                switch (towerData.TargettingRule)
                {
                    case TargettingRule.Nearest: // 타워와 가장 가까운 아군
                        towerInRange = towerInRange
                            .OrderBy(a => Vector2.Distance(transform.position, a.transform.position))
                            .ToList();
                        break;

                    case TargettingRule.Farthest: // 타워와 가장 먼 아군
                        towerInRange = towerInRange
                            .OrderByDescending(a => Vector2.Distance(transform.position, a.transform.position))
                            .ToList();
                        break;

                    default:
                        break;
                }
                break;

            default:
                break;
        }

        // 3. 타겟 수 제한
        int maxCount;

        switch (towerData.TargetType)
        {
            case TargetType.Enemy:
                //maxCount = Mathf.Min(towerData.targetCount, enemiesInRange.Count);
                maxCount = Mathf.Min((int)towerData.GetStatValue(TowerStatType.TargetCount), enemiesInRange.Count);
                for (int i = 0; i < maxCount; i++)
                {
                    targets.Add(enemiesInRange[i].gameObject);
                }
                break;

            case TargetType.Tower:
                maxCount = Mathf.Min((int)towerData.GetStatValue(TowerStatType.TargetCount), towerInRange.Count);
                for (int i = 0; i < maxCount; i++)
                {
                    targets.Add(towerInRange[i].gameObject);
                }
                break;

            default:
                break;
        }
    }

    protected override void Activate()
    {
        FindTargets();

        if (targets.Count == 0)
        {
            Debug.Log("타겟이 없음");
        }
        
        if (targets.Count > 0 )
        {
            UseActOnTargets();
        }
           
    }

    protected abstract void UseActOnTargets(); // 공격/버프 등을 하위에서 정의

    //버프 적용메서드
    private Dictionary<BuffEffectType, Coroutine> activeBuffs = new();

    public void ApplyBuff(BuffEffectType type, float amount, float duration)
    {
        // 이미 해당 타입의 버프가 있다면 중단
        if (activeBuffs.ContainsKey(type))
        {
            StopCoroutine(activeBuffs[type]);
            activeBuffs.Remove(type);
        }

        Coroutine newBuff = StartCoroutine(BuffCoroutine(type, amount, duration));
        activeBuffs[type] = newBuff;
    }

    private IEnumerator BuffCoroutine(BuffEffectType type, float amount, float duration)
    {
        int index = towerData.statTypes.FindIndex(v => (BuffEffectType)v == type);

        if (index == -1)
        {
            // 기존에 해당 타입이 없다면 새로 추가
            towerData.statTypes.Add((int)type);
            towerData.statValue.Add(amount);
        }
        else
        {
            // 기존 값에 더하기
            towerData.statValue[index] += amount;
        }

        yield return new WaitForSeconds(duration);

        // 버프 종료 시 복구
        if (index == -1)
        {
            // 추가했던 걸 다시 제거
            int lastIndex = towerData.statTypes.FindIndex(v => (BuffEffectType)v == type);
            if (lastIndex != -1)
            {
                towerData.statTypes.RemoveAt(lastIndex);
                towerData.statValue.RemoveAt(lastIndex);
            }
        }
        else
        {
            // 기존 값에서 차감
            towerData.statValue[index] -= amount;
        }
    }

}
