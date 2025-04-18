using System.Collections;
using UnityEngine;

public class DoTField : MonoBehaviour
{
    private float duration;
    private float damagePerTick;
    private float tickInterval;
    private float radius;

    private float timer;

    public SpriteRenderer visual; // 🌀 원형 시각 이펙트가 달린 오브젝트

    public void Init(float duration, float damagePerTick, float tickInterval, float radius)
    {
        this.duration = duration;
        this.damagePerTick = damagePerTick;
        this.tickInterval = tickInterval;
        this.radius = radius;

        // ✅ 시각 효과 스케일 조절
        if (visual != null)
        {
            visual.transform.localScale = Vector3.one * radius * 2f;
        }

        StartCoroutine(DamageTickRoutine());
        Destroy(gameObject, duration); // 일정 시간 후 장판 제거
    }

    private IEnumerator DamageTickRoutine()
    {
        while (true)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
            foreach (var hit in hits)
            {
                ISkillUser enemy = hit.GetComponent<ISkillUser>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damagePerTick);
                    Debug.Log($"🔥 {enemy.GetName()} 장판 피해 {damagePerTick}");
                }
            }

            yield return new WaitForSeconds(tickInterval);
            timer += tickInterval;

            if (timer >= duration)
                break;
        }
    }
}
