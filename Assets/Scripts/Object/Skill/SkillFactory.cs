using System;
using UnityEngine;

public static class SkillFactory
{
    public static Skill CreateSkill(SkillData data)
    {
        Type skillType = Type.GetType(data.skillClassName);
        if (skillType == null)
        {
            Debug.LogError($"❌ Skill 클래스 '{data.skillClassName}' 을(를) 찾을 수 없습니다.");
            return null;
        }

        Skill skill = Activator.CreateInstance(skillType) as Skill;
        if (skill == null)
        {
            Debug.LogError($"❌ Skill 클래스 '{data.skillClassName}' 는 Skill을 상속받지 않았습니다.");
            return null;
        }

        // 공통 필드 세팅
        skill.skillName = data.skillClassName;
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