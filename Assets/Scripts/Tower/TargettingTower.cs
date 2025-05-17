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

    protected override bool IsAttactAvailable()
    {
        FindTargets();
        return targets.Count > 0;
    }

    public virtual void FindTargets()
    {
        // targetingRule, targetType, targetCount 등을 이용해 타겟팅
        targets = new List<GameObject>();
        enemiesInRange = new List<MonsterBase>();
        towerInRange = new List<TargettingTower>();

        //범위 내 유닛 탐색
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, GetFinalStatValue(StatType.attackRange));        

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
                maxCount = Mathf.Min((int)GetFinalStatValue(StatType.targetCount), enemiesInRange.Count);
                for (int i = 0; i < maxCount; i++)
                {
                    targets.Add(enemiesInRange[i].gameObject);
                }
                break;

            case TargetType.Tower:
                maxCount = Mathf.Min((int)GetFinalStatValue(StatType.targetCount), towerInRange.Count);
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
        if (targets.Count == 0)
        {
            Debug.Log("타겟이 없음");
        }
        
        if (targets.Count > 0 )
        {
            UseActOnTargets();

            // 공격 애니메이션 재생
            anim?.SetTrigger("Attack");
            // 공격 방향
            Vector2 dir = (targets[0].transform.position - transform.position).normalized;
            if (spriteRenderer)
                spriteRenderer.flipX = dir.x < 0 ? true : false;
        }
    }

    protected abstract void UseActOnTargets(); // 공격/버프 등을 하위에서 정의

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, GetFinalStatValue(StatType.attackRange));
    }
}
