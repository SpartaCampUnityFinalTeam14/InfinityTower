using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Projectile_DoTField : Projectile
{
    private float dotDuration;
    private float dotDamagePerTick;
    private float dotTickInterval;
    private float dotRadius;

    private Transform target;
    private Vector3 lastKnownPosition;
    private bool targetLost = false;

    public override void Init(ProjectileData data, ProjectileDataSO visual, float customDamage, BaseTower towerData)
    {
        base.Init(data, visual, customDamage, towerData);

        // 🧠 ProjectileData에서 장판 세팅
        dotDuration = data.dotDuration;
        dotDamagePerTick = Mathf.Max(1, towerData.GetFinalStatValue(StatType.attackDamage) * 0.1f);
        dotTickInterval = data.dotTickInterval;
        dotRadius = data.dotRadius;
    }

    public override void SetTarget(Transform target)
    {
        this.target = target;
        if (target != null)
            lastKnownPosition = target.position;
    }

    protected override void Move()
    {
        if (!targetLost && target != null)
        {
            var monster = target.GetComponent<MonsterBase>();
            if (monster == null || monster.IsDead)
            {
                targetLost = true;
                target = null;
            }
            else
            {
                lastKnownPosition = target.position;
            }
        }

        Vector3 dir = (lastKnownPosition - transform.position).normalized;

        FlipByDirection(dir);
        
        Vector3 moveDir = dir * speed * Time.deltaTime;

        //float dist = Vector3.Distance(transform.position, targetPos);
        if (Vector3.SqrMagnitude(transform.position - lastKnownPosition) < moveDir.sqrMagnitude)
        {
            Explode();
        }
        else transform.position += moveDir;
    }

    void Explode()
    {
        Debug.Log("💥 장판 생성됨!");

        if (impactEffect != null)
        {
            GameObject field = Instantiate(impactEffect, transform.position, Quaternion.identity);

            DoTField dot = field.GetComponent<DoTField>();
            if (dot != null)
            {
                dot.Init(dotDuration, dotDamagePerTick, dotTickInterval, dotRadius);
                PoolManager.Instance.Release(this.GetComponent<Poolable>());
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
    }
}