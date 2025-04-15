using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SupportTower : TargettingTower
{
    public BuffType buffType;
    public float buffAmount;
    public float buffDuration;

    public DebuffType debuffType;
    public float debuffAmount;
    public float debuffDuration;

    protected override void UseActOnTargets()
    {
        SupportTowerData data = towerData as SupportTowerData;
        if (data == null) return;

        // 적 디버프 or 아군 버프 부여
        foreach (GameObject target in targets)
        {
            if (towerData.TargetType == TargetType.Enemy)
            {
                // 적에게 디버프
                MonsterBase enemy = target.GetComponent<MonsterBase>();
                if (enemy != null)
                {
                    enemy.ApplyDebuff(debuffType, debuffAmount, debuffDuration);
                }
            }
            else if (towerData.TargetType == TargetType.Tower)
            {
                // 아군에게 버프
                TargettingTower ally = target.GetComponent<TargettingTower>();
                if (ally != null)
                {
                    ally.ApplyBuff(buffType, debuffAmount, buffDuration);
                }
            }
        }
    }
}

