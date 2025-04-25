using UnityEngine;
using System.Collections.Generic;

public class Hero : ISkillUser
{
    public string heroName;
    public List<Skill> skills = new();
    public Vector3 fakePosition = Vector3.zero; 

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

}
