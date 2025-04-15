using UnityEngine;

public class HeroSkillPanel : MonoBehaviour
{
    public GameObject skillSlotPrefab;
    public Transform skillListParent;

    public void InitHero(Hero hero)
    {
        Debug.Log("🦸‍♂️ 영웅 스킬 패널 초기화");
        Debug.Log($"총 스킬 개수: {hero.skills.Count}");

        foreach (Skill skill in hero.skills)
        {
            Debug.Log($"🦸‍♂️ 스킬: {skill.skillName}");
            GameObject slotObj = Instantiate(skillSlotPrefab, skillListParent);
            SkillSlotUI slotUI = slotObj.GetComponent<SkillSlotUI>();
            slotUI.Init(skill, hero);
        }
    }
}