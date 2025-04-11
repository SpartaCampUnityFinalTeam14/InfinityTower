using UnityEngine;
using System.Collections.Generic;

public abstract class AutoTargetSkill : ActiveSkill
{
    public int maxTargets = 3;
    public float range = 5f;

    public override void Trigger(ISkillUser caster)
    {
        var targets = FindEnemiesNear(caster, range, maxTargets);

        foreach (var t in targets)
        {
            float damage = CalculateDamage(caster, t);
            t.TakeDamage(damage);
            Debug.Log($"⚡ {t.GetName()}에게 {damage} 피해!");
        }

        Debug.Log($"⚡ {skillName} 자동 대상 스킬 발동!");
    }

    // 각 스킬마다 데미지 계산만 다르게
    protected abstract float CalculateDamage(ISkillUser caster, ISkillUser target);
}