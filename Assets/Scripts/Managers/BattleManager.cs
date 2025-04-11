using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    
    public HeroSkillPanel skillPanel;

    private void Start()
    {
        Hero hero = new Hero();
        Debug.Log("👤 챔피언 생성됨: " + "djWJrnwjRJrn");

        // 임시로 챔피언 ID = 0 사용
        ChampionData champData = DataManager.Instance.championDict[0];
        Debug.Log(champData.skillID.Count + " 개의 스킬을 가지고 있습니다.");
        foreach (int skillId in champData.skillID)
        {
            if (DataManager.Instance.skillDict.TryGetValue(skillId, out SkillData skillData))
            {
                Debug.Log("⚡ 스킬 ID: " + skillId);
                Skill skill = SkillFactory.CreateSkill(skillData);
                if (skill != null)
                    hero.skills.Add(skill);
            }
            else
            {
                Debug.LogWarning($"⚠️ SkillData ID {skillId} 를 찾을 수 없습니다.");
            }
        }

        skillPanel.InitHero(hero);
    }
}