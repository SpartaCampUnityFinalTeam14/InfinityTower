using UnityEngine;

public class MeteorEffect : MonoBehaviour
{
    // 🔥 설정값들
    public float speed = 5f;
    public float damageRadius = 2.5f;
    public float damage = 30f;
    public float multiplier = 1f;

    // 🔧 내부 변수들
    private Vector3 targetPos;
    private ISkillUser caster;
    private GameObject explosionEffect;
    private Rigidbody2D rb;
    private bool hasExploded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector3 target, ISkillUser caster, GameObject explosionEffect, float radius, float dmg, float multi)
    {
        this.targetPos = target;
        this.caster = caster;
        this.explosionEffect = explosionEffect;
        this.damageRadius = radius;
        this.damage = dmg;
        this.multiplier = multi;

        // 🎯 시작 위치와 방향 설정
        Vector3 start = target + Vector3.up * 8f;
        transform.position = start;

        Vector2 dir = (target - start).normalized;
        rb.velocity = dir * speed;
    }

    private void Update()
    {
        if (hasExploded) return;

        float distance = Vector3.Distance(transform.position, targetPos);
        if (distance <= 0.2f || Vector3.Dot((targetPos - transform.position).normalized, rb.velocity.normalized) < 0f)
        {
            Explode();
        }
    }

    void Explode()
    {
        hasExploded = true;

        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        float finalDamage = damage * multiplier;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, damageRadius);
        foreach (var hit in hits)
        {
            ISkillUser target = hit.GetComponent<ISkillUser>();
            if (target != null)
            {
                target.TakeDamage(finalDamage);
                Debug.Log($"💥 {target.GetName()}에게 {finalDamage} 피해!");
            }
        }

        Destroy(gameObject); // 메테오 제거
    }
}
