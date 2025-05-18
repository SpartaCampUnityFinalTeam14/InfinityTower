using System;
using UnityEngine;

public static class SkillFactory
{
    public static Skill CreateSkill(SkillData data, float atk)
    {
        Skill skill = data.skillType switch
        {
            SkillType.AutoTarget => new AutoTargetDamage(),
            SkillType.TargetPosition => new TargetPositionDamage(),
            // SkillType.BossBuff => new BossBuffSkill(),
            _ => null
        };

        Debug.Log(0);

        if (skill == null)
        {
            Debug.LogError($"❌ 알 수 없는 SkillType: {data.skillType}");
            return null;
        }

        Debug.Log(1);

        // 공통 필드 세팅
        skill.skillName = data.visualId;
        skill.description = data.description;
        skill.baseDamage = atk;

        Debug.Log(2);

        // ✅ Effect 설정 (모든 스킬 공통 적용)
        if (data.effectID != null && data.effectID.Count > 0)
        {
            int effectId = data.effectID[0];
            float[] values = data.effectValue[0].values;

            int targetStatusID = DataManager.Instance.effectDict[effectId].targetStatusID;
            EffectBase effect = DataManager.Instance.effectDict[effectId].ReturnEffect(targetStatusID);

            skill.effectToApply = effect;
            skill.effectValue = values[0];
            skill.effectDuration = values[1];
            skill.stackable = values[2] != 0;
        }

        if (skill is ActiveSkill active)
        {
            active.cooldown = data.coolTime;
            active.attackType = data.attackType;
            
            if (active is TargetPositionSkill tp)
                tp.range = data.range;
        }

        Debug.Log(3);

        // 시각 연출용 SO 가져오기
        SkillVisualDataSO visual = StageManager.Instance.skillVisualDB.Get(data.visualId);
        //if (!string.IsNullOrEmpty(data.visualId))
        //{
        //    //if (SkillVisualDB.Instance == null)
        //    //    Debug.LogError("❌ SkillVisualDB.Instance is NULL!");

        //    visual = StageManager.Instance.skillVisualDB.Get(data.visualId);
        //}

        Debug.Log(4);

        // 💡 각 스킬 내부에서 알아서 처리하게
        skill.Setup(data, visual);

        Debug.Log(5);

        return skill;
    }
}