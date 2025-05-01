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
        //float duration = towerData.GetBuffValue(EffectType.Duration); // 지속시간

        //bool isAttackTower = towerData.statTypes.Contains((int)TowerStatType.Damage);

        foreach (GameObject target in targets)
        {
            if (target == null) continue;

            if (towerData.TargetType == TargetType.Enemy)
            {
                MonsterBase enemy = target.GetComponent<MonsterBase>();
                if (enemy == null) continue;

                if (enemy != null/*isAttackTower*/)
                {
                    float damage = towerData.GetStatValue(StatType.attackDamage);
                    float attackPowerBuff = GetAddModifierValue(StatType.attackDamage);
                    float totalDamage = GetFinalStatValue(StatType.attackDamage);

                    enemy.TakeDamage(totalDamage);
                    Debug.Log($"[hitScanTower] 데미지 {totalDamage} 적용 (기본:{damage}, 추가:{attackPowerBuff}) -> {enemy.name}");
                }

                foreach (EffectBase T in myEffect)
                {
                    if (T is not EffectBase_Monster monsterEffect)
                    {
                        continue;
                    }
                    float[] effectValues = towerData.effectInfo[towerData.effectID.IndexOf(T.statusID)];
                    monsterEffect.ApplyEffect_Monster(enemy, effectValues[0], effectValues[1], effectValues[2] > 0);
                }
                //// 디버프는 항상 적용
                //foreach (int rawType in towerData.effectID)
                //{
                //    EffectType effectType = (EffectType)rawType;
                //    if (effectType == EffectType.Duration) continue;

                //    float amount = towerData.GetBuffValue(effectType);
                //    enemy.ApplyDebuff(effectType, amount, duration);
                //    Debug.Log($"[hitScanTower] 디버프 {effectType} {amount} 적용 (지속: {duration}) -> {enemy.name}");
                //}
            }
            else if (towerData.TargetType == TargetType.Tower)
            {
                TargettingTower ally = target.GetComponent<TargettingTower>();
                foreach (EffectBase T in myEffect)
                {
                    if (T is not EffectBase_Tower towerEffect)
                    {
                        continue;
                    }
                    float[] effectValues = towerData.effectInfo[towerData.effectID.IndexOf(T.statusID)];
                    towerEffect.ApplyEffect_Tower(ally, effectValues[0], effectValues[1], effectValues[2] > 0);
                }

                //// 아군 버프
                //if (isAttackTower) continue; // 공격력 가진 타워는 아군을 버프하지 않음

                //TargettingTower ally = target.GetComponent<TargettingTower>();
                //if (ally != null && ally != this)
                //{
                //    foreach (int rawType in towerData.effectID)
                //    {
                //        EffectType effectType = (EffectType)rawType;
                //        if (effectType == EffectType.Duration) continue;

                //        float amount = towerData.GetBuffValue(effectType);
                //        ally.ApplyBuff(effectType, amount, duration);
                //        Debug.Log($"[hitScanTower] 버프 {effectType} {amount} 적용 (지속: {duration}) -> {ally.name}");
                //    }
                //}
            }
        }
    }
}

