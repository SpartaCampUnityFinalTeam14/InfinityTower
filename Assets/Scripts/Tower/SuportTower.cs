using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuportTower : TargettingTower
{
    protected override void UseActOnTargets()
    {
        // 적 디버프 or 아군 버프 부여
        foreach (GameObject target in targets)
        {
            if (towerData.TargetType == TargetType.Enemy)
            {
                // 적에게 디버프
                MonsterBase enemy = target.GetComponent<MonsterBase>();
                if (enemy != null)
                {
                    enemy.ApplyDebuff(towerData.DebuffType, towerData.debuffAmount, towerData.debuffDuration);
                }
            }
            else if (towerData.TargetType == TargetType.Ally)
            {
                // 아군에게 버프
                TargettingTower ally = target.GetComponent<TargettingTower>();
                if (ally != null)
                {
                    ally.ApplyBuff(towerData.BuffType, towerData.buffAmount, towerData.buffDuration);
                }
            }
        }
    }
}

