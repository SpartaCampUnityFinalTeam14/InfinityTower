using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public abstract class TargettingTower : BaseTower
{
    protected List<GameObject> targets;

    public virtual void FindTargets()
    {
        // targetingRule, targetType, targetCount 등을 이용해 타겟팅
        targets = new List<GameObject>();

        //범위 내 유닛 탐색
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, towerData.range);

        //범위 안에 들어온 적 리스트
        List<Enemy> enemiesInRange = new List<Enemy>();

        //범위 안에 있는 아군 타워 리스트
        List<TargettingTower> allyTower = new List<TargettingTower>();

        foreach (Collider2D hit in hits)
        {
            //범위 안에 있는 적과 아군 판별
            if (hit.CompareTag("Enemy"))
            {
                enemiesInRange.Add(hit.GetComponent<Enemy>());
            }

            if (hit.CompareTag("Tower"))
            {
                allyTower.Add(hit.GetComponent<TargettingTower>());
            }
        }

        // 2. 타겟팅 룰 적용
        switch (towerData.targetingRule)
        {
            case TargetingRule.Nearest:
                float nearestDistance = float.MaxValue;
                foreach (Enemy e in enemiesInRange)
                {
                    float distance = Vector2.Distance(transform.position, e.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        target = e;
                    }
                }
                break;

            case TargetingRule.Farthest:
                float farthestDistance = float.MinValue;
                foreach (Enemy e in enemiesInRange)
                {
                    float distance = Vector2.Distance(transform.position, e.transform.position);
                    if (distance > farthestDistance)
                    {
                        farthestDistance = distance;
                        target = e;
                    }
                }
                break;

            case TargetingRule.LowestHP:
                float lowestHP = float.MaxValue;
                foreach (Enemy e in enemiesInRange)
                {
                    if (e.currentHP < lowestHP)
                    {
                        lowestHP = e.currentHP;
                        target = e;
                    }
                }
                break;

            case TargetingRule.HighestHP:
                float highestHP = float.MinValue;
                foreach (Enemy e in enemiesInRange)
                {
                    if (e.currentHP > highestHP)
                    {
                        highestHP = e.currentHP;
                        target = e;
                    }
                }
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
