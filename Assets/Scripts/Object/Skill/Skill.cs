using System.Collections.Generic;
using UnityEngine;

public interface ISkillUser
{
    string GetName();
    void TakeDamage(float amount);
    Vector3 GetPosition(); // 추가!
}


public abstract class Skill
{
    public string skillName;
    public string description;
    

    public abstract void Use(ISkillUser caster);
    
    // 🧠 SkillData 및 시각 리소스 주입용 (선택적 override)
    public virtual void Setup(SkillData data, SkillVisualDataSO visual) { }
    
    public List<ISkillUser> FindEnemiesNear(ISkillUser caster, float range, int max = 999)
    {
        Vector3 casterPos = caster.GetPosition();

        List<ISkillUser> result = new();
        Collider2D[] hits = Physics2D.OverlapCircleAll(casterPos, range);

        foreach (var h in hits)
        {
            ISkillUser target = h.GetComponent<ISkillUser>();
            if (target != null && target != caster)
                result.Add(target);
        }

        return result.Count > max ? result.GetRange(0, max) : result;
    }


    public List<ISkillUser> FindEnemiesInArea(Vector3 center, float radius)
    {
        List<ISkillUser> results = new();

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);

        foreach (var h in hits)
        {
            ISkillUser target = h.GetComponent<ISkillUser>();
            if (target != null)
                results.Add(target);
        }

        return results;
    }
    
    public List<ISkillUser> FindNearestEnemies(ISkillUser caster, int maxCount)
    {
        Vector3 casterPos = caster.GetPosition();

        List<ISkillUser> result = new();
        Collider2D[] hits = Physics2D.OverlapCircleAll(casterPos, 100f); // 📌 넉넉한 범위로 모두 탐색

        foreach (var h in hits)
        {
            ISkillUser target = h.GetComponent<ISkillUser>();
            if (target != null && target != caster)
            {
                result.Add(target);
            }
        }

        // 거리순 정렬 후 상위 maxCount 개만 추출
        result.Sort((a, b) =>
            Vector3.Distance(casterPos, a.GetPosition())
                .CompareTo(Vector3.Distance(casterPos, b.GetPosition())));

        return result.GetRange(0, Mathf.Min(maxCount, result.Count));
    }
}
