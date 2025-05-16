using UnityEngine;

public class AutoTargetDamage : AutoTargetSkill
{
    public float multiplier = 1f;

    // 🎇 연출용
    public GameObject effectPrefab;
    public AudioClip soundClip;

    protected override float CalculateDamage(ISkillUser caster, ISkillUser target)
    {
        return baseDamage * multiplier;
    }

    public override void Trigger(ISkillUser caster)
    {
        var targets = FindNearestEnemies(caster, maxTargets);

        float finalDamage = CalculateDamage(caster, null); // 타겟에 따라 다르게 하려면 수정
        foreach (var target in targets)
        {
            target.TakeDamage(finalDamage);
            Debug.Log($"⚡ {target.GetName()}에게 {finalDamage} 피해!");

            // ✅ 이펙트 - 타겟 위치에 생성
            if (effectPrefab != null)
                GameObject.Instantiate(effectPrefab, target.GetPosition(), Quaternion.identity);
        }

        // ✅ 사운드 - 중심 위치에서 재생
        if (soundClip != null)
            AudioSource.PlayClipAtPoint(soundClip, caster.GetPosition());

        Debug.Log($"⚡ {caster.GetName()}이(가) 체인 라이트닝 발동!");
    }
    
    public override void Setup(SkillData data, SkillVisualDataSO visual)
    {
        multiplier = data.multiplier;

        if (visual != null)
        {
            effectPrefab = visual.effectPrefab;
            soundClip = visual.soundClip;
        }
    }

}