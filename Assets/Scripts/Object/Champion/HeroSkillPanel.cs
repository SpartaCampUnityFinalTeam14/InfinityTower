using UnityEngine;

public class HeroSkillPanel : MonoBehaviour
{
    public GameObject skillSlotPrefab;
    public Transform skillListParent;

    private void Start()
    {
        InitHero();
    }

    void InitHero()
    {
        StageManager.Instance.CurHero = new Hero();
        //Debug.Log("👤 챔피언 생성됨: " + SaveManager.Instance.playerData.selectedChampionIndex);

        int championId = SaveManager.Instance.playerData.selectedChampionIndex;
        ChampionData champData = DataManager.Instance.championDict[championId];

        //Debug.Log($"{champData.skillId.Count} 개의 스킬을 가지고 있습니다.");

        foreach (int skillId in champData.skillId)
        {
            if (DataManager.Instance.skillDict.TryGetValue(skillId, out SkillData skillData))
            {
                Debug.Log($"⚡ 스킬 ID: {skillId}");
                Skill skill = SkillFactory.CreateSkill(skillData, champData.atk);
                if (skill != null)
                    StageManager.Instance.CurHero.skills.Add(skill);
            }
            else
            {
                Debug.LogWarning($"⚠️ SkillData ID {skillId} 를 찾을 수 없습니다.");
            }
        }

        InitHero(StageManager.Instance.CurHero);
    }

    public void InitHero(Hero hero)
    {
        foreach(Transform child in skillListParent)
        {
            Destroy(child.gameObject);
        }

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