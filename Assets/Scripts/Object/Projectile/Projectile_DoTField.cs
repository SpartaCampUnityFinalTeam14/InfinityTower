using UnityEngine;

public class Projectile_DoTField : Projectile
{
    // 👇 장판 관련 설정값
    private float dotDuration;
    private float dotDamagePerTick;
    private float dotTickInterval;
    private float dotRadius;

    private Vector3 targetPos;
    private bool hasExploded = false;

    public override void Init(ProjectileDataSO data)
    {
        base.Init(data);
        dotDuration = data.dotDuration;
        dotDamagePerTick = data.dotDamagePerTick;
        dotTickInterval = data.dotTickInterval;
        dotRadius = data.dotRadius;
    }

    public override void SetTarget(Transform t)
    {
        if (t != null)
            targetPos = t.position; // 🧠 타겟이 사라져도 위치는 기억
    }

    protected override void Move()
    {
        if (hasExploded) return;

        Vector3 dir = (targetPos - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        float dist = Vector3.Distance(transform.position, targetPos);
        if (dist < 0.1f)
        {
            Explode();
        }
    }

    void Explode()
    {
        hasExploded = true;
        Debug.Log("💥 장판 생성됨!");

        if (impactEffect != null) // ✅ SO에 연결된 프리팹 사용
        {
            GameObject field = Instantiate(impactEffect, transform.position, Quaternion.identity);

            DoTField dot = field.GetComponent<DoTField>();
            if (dot != null)
            {
                dot.Init(dotDuration, dotDamagePerTick, dotTickInterval, dotRadius);
            }
            else
            {
                Debug.LogWarning("⚠️ DoTField 스크립트가 impactEffect에 없음!");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ impactEffect가 비어있음!");
        }

        Destroy(gameObject);
    }

}