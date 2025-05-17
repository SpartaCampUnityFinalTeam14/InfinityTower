using UnityEngine;
using System.Collections.Generic;

public class Hero : ISkillUser
{
    public string heroName;
    public List<Skill> skills = new();
    public Vector3 fakePosition = Vector3.zero; 
    
    private float baseAtk;
    private float artifactBonus ;

    public void UseSkill(int index)
    {
        if (index < 0 || index >= skills.Count) return;
        skills[index].Use(this); // 핵심
    }

    public void TakeDamage(float dmg) 
    { 
        // 피해 처리 로직
        Debug.Log($"{heroName}이(가) {dmg}의 피해를 받았습니다.");
    }
    public string GetName() => heroName;
    public Vector3 GetPosition() => fakePosition;
    public int GetTeam()
    {
        return 1;
    }

    public float GetBaseDamage()
    {
        //여기서 최종 데미지 결정해야될듯? 유물 특성 + 
        return baseAtk + artifactBonus;
    }

    // ✅ 여기서 모든 스킬 데미지를 갱신
    public void RefreshSkillBaseDamage()
    {
        float dmg = GetBaseDamage();
        foreach (var s in skills)
            s.baseDamage = dmg;
    }

    // 예: 외부에서 호출
    public void AddStatFromArtifact(float value)
    {
        artifactBonus = value;
        RefreshSkillBaseDamage();
    }
}
