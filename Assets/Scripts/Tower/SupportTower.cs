using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SupportTower : TargettingTower
{
    protected override void UseActOnTargets()
    {
        float duration = towerData.GetValue(BuffEffectType.Duration); // 지속시간

        foreach (GameObject target in targets)
        {
            foreach (int rawType in towerData.valueTypes)
            {
                BuffEffectType effectType = (BuffEffectType)rawType;

                // 지속시간 타입은 적용 효과가 아니므로 패스
                if (effectType == BuffEffectType.Duration) continue;

                float amount = towerData.GetValue(effectType);

                if (towerData.TargetType == TargetType.Enemy)
                {
                    // 디버프 적용
                    MonsterBase enemy = target.GetComponent<MonsterBase>();
                    if (enemy != null)
                    {
                        enemy.ApplyDebuff(effectType, amount, duration);
                    }
                }
                else if (towerData.TargetType == TargetType.Tower)
                {
                    // 버프 적용
                    TargettingTower ally = target.GetComponent<TargettingTower>();
                    if (ally != null)
                    {
                        ally.ApplyBuff(effectType, amount, duration);
                    }
                }
            }
        }
    }
}

