using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SuportTower : TargettingTower
{
    public int suportId;
    // 버프/디버프 데이터 추가
    public int buffType;
    public float buffAmount;
    public float buffDuration;

    public int debuffType;
    public float debuffAmount;
    public float debuffDuration;
    public BuffType BuffType => (BuffType)buffType;
    public DebuffType DebuffType => (DebuffType)debuffType;

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
                    enemy.ApplyDebuff(DebuffType, debuffAmount, debuffDuration);
                }
            }
            else if (towerData.TargetType == TargetType.Ally)
            {
                // 아군에게 버프
                TargettingTower ally = target.GetComponent<TargettingTower>();
                if (ally != null)
                {
                    ally.ApplyBuff(BuffType, debuffAmount, buffDuration);
                }
            }
        }
    }
}

