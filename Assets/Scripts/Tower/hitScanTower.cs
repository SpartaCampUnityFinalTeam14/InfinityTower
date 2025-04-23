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
        float duration = towerData.GetBuffValue(BuffEffectType.Duration); // 지속시간

        bool isAttackTower = towerData.statTypes.Contains((int)TowerStatType.Damage);

        foreach (GameObject target in targets)
        {
            if (target == null) continue;

            if (towerData.TargetType == TargetType.Enemy)
            {
                MonsterBase enemy = target.GetComponent<MonsterBase>();
                if (enemy == null) continue;

                if (isAttackTower)
                {
                    float damage = towerData.GetStatValue(TowerStatType.Damage);
                    float attackPowerBuff = towerData.GetBuffValue(BuffEffectType.ATKPowerUP);
                    float totalDamage = damage + attackPowerBuff;

                    enemy.TakeDamage(totalDamage);
                    Debug.Log($"[hitScanTower] 데미지 {totalDamage} 적용 (기본:{damage}, 추가:{attackPowerBuff}) -> {enemy.name}");
                }

                // 디버프는 항상 적용
                foreach (int rawType in towerData.effectID)
                {
                    BuffEffectType effectType = (BuffEffectType)rawType;
                    if (effectType == BuffEffectType.Duration) continue;

                    float amount = towerData.GetBuffValue(effectType);
                    enemy.ApplyDebuff(effectType, amount, duration);
                    Debug.Log($"[hitScanTower] 디버프 {effectType} {amount} 적용 (지속: {duration}) -> {enemy.name}");
                }
            }
            else if (towerData.TargetType == TargetType.Tower)
            {
                // 아군 버프
                if (isAttackTower) continue; // 공격력 가진 타워는 아군을 버프하지 않음

                TargettingTower ally = target.GetComponent<TargettingTower>();
                if (ally != null && ally != this)
                {
                    foreach (int rawType in towerData.effectID)
                    {
                        BuffEffectType effectType = (BuffEffectType)rawType;
                        if (effectType == BuffEffectType.Duration) continue;

                        float amount = towerData.GetBuffValue(effectType);
                        ally.ApplyBuff(effectType, amount, duration);
                        Debug.Log($"[hitScanTower] 버프 {effectType} {amount} 적용 (지속: {duration}) -> {ally.name}");
                    }
                }
            }

            //foreach (int rawType in towerData.effectID)
            //{
            //    BuffEffectType effectType = (BuffEffectType)rawType;

            //    // 지속시간 타입은 적용 효과가 아니므로 패스
            //    if (effectType == BuffEffectType.Duration) continue;

            //    float amount = towerData.GetBuffValue(effectType);

            //    if (towerData.TargetType == TargetType.Enemy)
            //    {
            //        MonsterBase enemy = target.GetComponent<MonsterBase>();
            //        if (enemy == null) continue;

            //        if (effectType == BuffEffectType.Damage)
            //        {
            //            float baseDamage = towerData.GetBuffValue(BuffEffectType.Damage);
            //            float attackPowerBuff = towerData.GetBuffValue(BuffEffectType.AttackPower);
            //            float totalDamage = baseDamage + attackPowerBuff;

            //            // 공격 적용
            //            enemy.TakeDamage(totalDamage);
            //            Debug.Log($"[hitScanTower] 데미지 {totalDamage} 적용 (기본:{baseDamage}, 추가:{attackPowerBuff}) -> {enemy.name}");
            //        }
            //        else
            //        {
            //            // 디버프 적용
            //            enemy.ApplyDebuff(effectType, amount, duration);
            //            Debug.Log($"[hitScanTower] 디버프 {effectType} {amount} 적용 (지속: {duration}) -> {enemy.name}");
            //        }
            //    }
            //    else if (towerData.TargetType == TargetType.Tower)
            //    {
            //        // 버프 적용
            //        TargettingTower ally = target.GetComponent<TargettingTower>();
            //        if (ally != null && ally != this) //자기 자신 제외
            //        {
            //            ally.ApplyBuff(effectType, amount, duration);
            //            Debug.Log($"[SupportTower] 버프 {effectType} {amount} 적용 (지속: {duration}) -> {ally.name}");
            //        }
            //    }
            //}
        }
    }
}

