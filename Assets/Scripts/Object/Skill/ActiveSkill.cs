using System.Collections.Generic;
using UnityEngine;

public abstract class ActiveSkill : Skill
{
    public float attackType;
    public Dictionary<int,EffectBase> myEffectDict;
    
    public float baseCooldown;
    public float cooldownModifier = 1f;
    
    private float totalCooldownReduction = 0f;
    public float cooldown => baseCooldown * cooldownModifier;
    public float RemainingCooldown => Mathf.Max(0, lastUsedTime + cooldown - Time.time);
    
    protected float lastUsedTime = -999; // 마지막 사용 시각 (Time.time 기준)

    public bool CanUse()
    {
        return Time.time >= lastUsedTime + cooldown;
    }

    public override void Use(ISkillUser caster)
    {
        if (!CanUse())
        {
            Debug.LogWarning($"⛔ {skillName}은(는) 쿨타임 중입니다. 남은 시간: {RemainingCooldown:F1}s");
            return;
        }

        lastUsedTime = Time.time;
        Trigger(caster);
    }

    public abstract void Trigger(ISkillUser caster);

    // 외부에서 호출 시 쿨타임 조절용도
    public void ApplyCooldownReduction(float percent)
    {
        totalCooldownReduction += percent;
        totalCooldownReduction = Mathf.Clamp01(totalCooldownReduction); // 최대 100% 감소 방지

        cooldownModifier = Mathf.Max(0.1f, 1f - totalCooldownReduction); // 최소 10% 유지
    }
}