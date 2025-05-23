using UnityEngine;

public class TargetPositionDamage : TargetPositionSkill
{
    public float multiplier;
    public float radius;

    public GameObject effectPrefab;
    public GameObject explosionEffect;
    public AudioClip soundClip;

    public override void Trigger(ISkillUser caster)
    {
        SkillTargetingSystem.Instance.StartTargeting(this, caster);
    }

    public override void ExecuteAt(Vector3 pos, ISkillUser caster)
    {
        if (effectPrefab != null)
        {
            GameObject meteor = GameObject.Instantiate(
                effectPrefab,
                pos + Vector3.up * 10f,
                effectPrefab.transform.rotation // ✅ 이걸로 교체!
            );

            TargetPositionEffect effect = meteor.GetComponent<TargetPositionEffect>();

            if (effect != null)
            {
                Debug.Log($"{baseDamage}");
                effect.Init(pos, caster, explosionEffect, radius, baseDamage, multiplier, attackType);
                
                effect.effectToApply   = this.effectToApply;     
                effect.effectValue     = this.effectValue * baseDamage;       
                effect.effectDuration  = this.effectDuration;    
                effect.stackable       = this.stackable;         
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
        radius = data.range;
        Debug.Log(effectPrefab != null);
        if (visual != null)
        {
            effectPrefab = visual.effectPrefab;
            explosionEffect = visual.explosionEffect;
            soundClip = visual.soundClip;
        }
    }
}
