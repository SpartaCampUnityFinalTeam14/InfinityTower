using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public abstract class TargettingTower : BaseTower
{
    protected List<GameObject> targets;
    //범위 안에 들어온 적 리스트
    List<Enemy> enemiesInRange;
    //범위 안에 있는 아군 타워 리스트
    List<TargettingTower> allyTower;

    public virtual void FindTargets()
    {
        // targetingRule, targetType, targetCount 등을 이용해 타겟팅
        targets = new List<GameObject>();

        //범위 내 유닛 탐색
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, towerData.range);

        enemiesInRange = new List<Enemy>();
        allyTower = new List<TargettingTower>();

        foreach (Collider2D hit in hits)
        {
            //범위 안에 있는 적과 아군 판별
            if (hit.CompareTag("Enemy"))
            {
                //enemiesInRange.Add(hit.GetComponent<Enemy>());
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                    enemiesInRange.Add(enemy);
            }

            if (hit.CompareTag("Tower"))
            {
                //allyTower.Add(hit.GetComponent<TargettingTower>());
                TargettingTower tower = hit.GetComponent<TargettingTower>();
                if (tower != null)
                    allyTower.Add(tower);
            }
        }

        // 2. 타겟팅 룰 적용
        switch (towerData.targetingRule)
        {
            case TargettingRule.Nearest: // 타워와 가장 가까운 적
                enemiesInRange = enemiesInRange
                    .OrderBy(e => Vector2.Distance(transform.position, e.transform.position))
                    .ToList();
                break;

            case TargettingRule.Farthest: // 타워와 가장 먼 적
                enemiesInRange = enemiesInRange
                    .OrderByDescending(e => Vector2.Distance(transform.position, e.transform.position))
                    .ToList();
                break;

            case TargettingRule.LowestHP: // 체력이 가장 낮은 적
                enemiesInRange = enemiesInRange
                    .OrderBy(e => e.currentHP)
                    .ToList();
                break;

            case TargettingRule.HighestHP: // 체력이 가장 높은 적
                enemiesInRange = enemiesInRange
                    .OrderByDescending(e => e.currentHP)
                    .ToList();
                break;

            default:
                break;
        }

        // 3. 타겟 수 제한
        int maxCount = Mathf.Min(towerData.targetCount, enemiesInRange.Count);
        for (int i = 0; i < maxCount; i++)
        {
            targets.Add(enemiesInRange[i].gameObject);
        }
    }
    public override void Activate()
    {
        FindTargets();
        if (targets.Count > 0)
            UseSkillOnTargets();
    }

    protected abstract void UseSkillOnTargets(); // 공격/버프 등을 하위에서 정의
}
