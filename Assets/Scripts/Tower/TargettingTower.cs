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
    List<TargettingTower> allyInRange;

    public virtual void FindTargets()
    {
        // targetingRule, targetType, targetCount 등을 이용해 타겟팅
        targets = new List<GameObject>();

        //범위 내 유닛 탐색
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, towerData.range);

        enemiesInRange = new List<MonsterBase>();
        allyInRange = new List<TargettingTower>();

        //범위 안에 있는 적과 아군 판별
        foreach (Collider2D hit in hits)
        {
            switch (towerData.TargetType)
            {
                case TargetType.Enemy:
                    if (hit.CompareTag("Enemy"))
                    {
                        //enemiesInRange.Add(hit.GetComponent<Enemy>());
                        MonsterBase enemy = hit.GetComponent<MonsterBase>();
                        if (enemy != null)
                            enemiesInRange.Add(enemy);
                    }
                break;

                case TargetType.Tower:
                    if (hit.CompareTag("Ally"))
                    {
                        //allyTower.Add(hit.GetComponent<TargettingTower>());
                        TargettingTower tower = hit.GetComponent<TargettingTower>();
                        if (tower != null)
                            allyInRange.Add(tower);
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
                        allyInRange = allyInRange
                            .OrderBy(a => Vector2.Distance(transform.position, a.transform.position))
                            .ToList();
                        break;

                    case TargettingRule.Farthest: // 타워와 가장 먼 아군
                        allyInRange = allyInRange
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
                maxCount = Mathf.Min(towerData.targetCount, enemiesInRange.Count);
                for (int i = 0; i < maxCount; i++)
                {
                    targets.Add(enemiesInRange[i].gameObject);
                }
                break;

            case TargetType.Tower:
                maxCount = Mathf.Min(towerData.targetCount, allyInRange.Count);
                for (int i = 0; i < maxCount; i++)
                {
                    targets.Add(allyInRange[i].gameObject);
                }
                break;

            default:
                break;
        }
    }

    public override void Activate()
    {
        FindTargets();

        if (targets.Count == 0)
        {
            //break;
        }
        
        if (targets.Count > 0 )
        {
            UseActOnTargets();
        }
           
    }

    protected abstract void UseActOnTargets(); // 공격/버프 등을 하위에서 정의

    //버프 적용메서드
    public void ApplyBuff(BuffEffectType type, float amount, float duration)
    {
        //중복 적용 안되게 스탑코루틴 작성
        StopCoroutine("BuffCoroutine");
        StartCoroutine(BuffCoroutine(type, amount, duration));
    }

    private IEnumerator BuffCoroutine(BuffEffectType type, float amount, float duration)
    {
        switch (type)
        {
            case BuffEffectType.AttackSpeed:
                towerData.coolTime = Mathf.Max(0.1f, towerData.coolTime - amount);
                break;

            case BuffEffectType.Range:
                towerData.range += amount;
                break;

            case BuffEffectType.CooldownReduction:
                cooldownTimer -= amount;
                break;
        }

        yield return new WaitForSeconds(duration);

        // 버프 종료 시 원래대로 복구
        switch (type)
        {
            case BuffEffectType.AttackSpeed:
                towerData.coolTime += amount;
                break;

            case BuffEffectType.Range:
                towerData.range -= amount;
                break;

            case BuffEffectType.CooldownReduction:
                // 이건 타이머에 영향을 주는 일회성이라 되돌릴 필요 없음
                break;
        }
    }
}
