using System;
using UnityEngine;

public static class SkillFactory
{
    public static Skill CreateSkill(SkillData data)
    {
        Skill skill = data.skillType switch
        {
            SkillType.AutoTarget => new AutoTargetDamage(),
            SkillType.TargetPosition => new TargetPositionDamage(),
            // SkillType.BossBuff => new BossBuffSkill(),
            _ => null
        };

        if (skill == null)
        {
            Debug.LogError($"❌ 알 수 없는 SkillType: {data.skillType}");
            return null;
        }

        // 공통 필드 세팅
        skill.skillName = data.visualId;
        skill.description = data.description;

        if (skill is ActiveSkill active)
        {
            active.cooldown = data.coolTime;
            if (active is TargetPositionSkill tp)
                tp.range = data.range;
        }

        // 시각 연출용 SO 가져오기
        SkillVisualDataSO visual = null;
        if (!string.IsNullOrEmpty(data.visualId))
        {
            if (SkillVisualDB.Instance == null)
                Debug.LogError("❌ SkillVisualDB.Instance is NULL!");

            visual = SkillVisualDB.Instance.Get(data.visualId);
        }

        // 💡 각 스킬 내부에서 알아서 처리하게
        skill.Setup(data, visual);

        return skill;
    }
}