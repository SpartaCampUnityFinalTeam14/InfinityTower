using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class hitScanTower : TargettingTower
{
    protected override void Update()
    {
        base.Update();
    }

    protected override void UseActOnTargets()
    {
        foreach (GameObject target in targets)
        {
            if (target == null) continue;
            //공격 적용
            if (towerData.TargetType == TargetType.Enemy)
            {
                MonsterBase enemy = target.GetComponent<MonsterBase>();
                if (enemy == null) continue;

                float damage = towerData.GetStatValue(StatType.attackDamage);
                float attackPowerBuff = GetAddModifierValue(StatType.attackDamage);
                float totalDamage = GetFinalStatValue(StatType.attackDamage);

                enemy.TakeDamage(totalDamage);
                Debug.Log($"[hitScanTower] 데미지 {totalDamage} 적용 (기본:{damage}, 추가:{attackPowerBuff}) -> {enemy.name}");
            }

            if (target == null) continue;
            //효과 적용
            ApplyEffectOnAttack(target);
        }
    }
}

