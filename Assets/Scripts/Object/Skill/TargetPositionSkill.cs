using System.Collections.Generic;
using UnityEngine;

public class TargetPositionSkill : ActiveSkill
{
    public float range;
    private Vector3 targetPosition;

    public void SetTarget(Vector3 pos)
    {
        targetPosition = pos;
    }

    public override void Trigger(ISkillUser caster)
    {
        ExecuteAt(targetPosition, caster);
    }

    public virtual void ExecuteAt(Vector3 pos, ISkillUser caster)
    {
        Debug.Log($"🎯 ExecuteAt 호출: {pos}");
        // 하위 클래스에서 구현
    }
    
    public bool TryStartSkill(ISkillUser caster, Vector3 position)
    {
        if (!CanUse())
        {
            Debug.LogWarning($"⛔ {skillName}은(는) 쿨타임 중입니다. 남은 시간: {RemainingCooldown:F1}s");
            return false;
        }

        lastUsedTime = Time.time;
        ExecuteAt(position, caster);
        return true;
    }
}