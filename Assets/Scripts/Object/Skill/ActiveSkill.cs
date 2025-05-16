using System.Collections.Generic;
using UnityEngine;

public abstract class ActiveSkill : Skill
{
    public float cooldown;               // 스킬 쿨타임 (초 단위)
    public float attackType;
    public Dictionary<int,EffectBase> myEffectDict;
    
    protected float lastUsedTime = -999; // 마지막 사용 시각 (Time.time 기준)

    public float RemainingCooldown => Mathf.Max(0, lastUsedTime + cooldown - Time.time);

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
}