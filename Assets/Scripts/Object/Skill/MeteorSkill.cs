using UnityEngine;

public class MeteorSkill : TargetPositionSkill
{
    public float multiplier;
    public float baseDamage = 20f;
    public float radius = 2.5f;

    public GameObject effectPrefab;
    public GameObject explosionEffect;
    public AudioClip soundClip;

    public override void Trigger(ISkillUser caster)
    {
        SkillTargetingSystem.Instance.StartTargeting(this, caster);
    }

    public override void ExecuteAt(Vector3 pos, ISkillUser caster)
    {
        Debug.Log("☄️ MeteorSkill.ExecuteAt");
        
        if (effectPrefab != null)
        {
            GameObject meteor = GameObject.Instantiate(effectPrefab, pos + Vector3.up * 10f, Quaternion.identity);
            MeteorEffect effect = meteor.GetComponent<MeteorEffect>();

            if (effect != null)
            {
                Debug.Log("✅ MeteorEffect 정상 연결됨");
                effect.Init(pos, caster, explosionEffect, radius, baseDamage, multiplier);
            }
            else
            {
                Debug.LogWarning("🚨 MeteorEffect 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.Log("🚨 이펙트 프리팹이 없습니다.");
        }

        if (soundClip != null)
            AudioSource.PlayClipAtPoint(soundClip, pos);
    }


    public override void Setup(SkillData data, SkillVisualDataSO visual)
    {
        multiplier = data.multiplier;
        Debug.Log(effectPrefab != null);
        if (visual != null)
        {
            effectPrefab = visual.effectPrefab;
            explosionEffect = visual.explosionEffect;
            soundClip = visual.soundClip;
        }
    }
}
