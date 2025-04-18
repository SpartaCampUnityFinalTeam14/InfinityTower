using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class hitScanTower : TargettingTower
{
    public override void Update()
    {
        base.Update();
    }

    protected override void UseActOnTargets()
    {
        float duration = towerData.GetValue(BuffEffectType.Duration); // 지속시간

        foreach (GameObject target in targets)
        {
            if (target == null) continue;

            foreach (int rawType in towerData.valueTypes)
            {
                BuffEffectType effectType = (BuffEffectType)rawType;

                // 지속시간 타입은 적용 효과가 아니므로 패스
                if (effectType == BuffEffectType.Duration) continue;

                float amount = towerData.GetValue(effectType);

                if (towerData.TargetType == TargetType.Enemy)
                {
                    MonsterBase enemy = target.GetComponent<MonsterBase>();
                    if (enemy == null) continue;

                    if (effectType == BuffEffectType.Damage)
                    {
                        // 공격 적용
                        enemy.TakeDamage(amount);
                        Debug.Log($"[hitScanTower] 데미지 {amount} 적용 -> {enemy.name}");
                    }
                    else
                    {
                        // 디버프 적용
                        enemy.ApplyDebuff(effectType, amount, duration);
                        Debug.Log($"[hitScanTower] 디버프 {effectType} {amount} 적용 (지속: {duration}) -> {enemy.name}");
                    }
                }
                else if (towerData.TargetType == TargetType.Tower)
                {
                    // 버프 적용
                    TargettingTower ally = target.GetComponent<TargettingTower>();
                    if (ally != null && ally != this) //자기 자신 제외
                    {
                        ally.ApplyBuff(effectType, amount, duration);
                        Debug.Log($"[SupportTower] 버프 {effectType} {amount} 적용 (지속: {duration}) -> {ally.name}");
                    }
                }
            }
        }
    }
}

