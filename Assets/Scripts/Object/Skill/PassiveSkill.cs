using UnityEngine;

public class PassiveSkill : Skill
{
    public float damage;

    public override void Use(ISkillUser caster)
    {
        Debug.Log($"🟢 {caster.GetName()}이(가) {skillName}을 사용!");
    }
}