using UnityEngine;

public class SplashProjectile : Projectile
{
    private Transform target;
    private Vector3 lastKnownPosition;
    private bool targetLost = false;

    public float splashRadius;

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
            }
            else
            {
                lastKnownPosition = target.position;
            }
        }

        Vector3 dir = (lastKnownPosition - transform.position).normalized;
        
        FlipByDirection(dir);
        
        transform.position += dir * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, lastKnownPosition) < 0.1f)
        {
            Explode();
        }
    }

    private void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, splashRadius);
        foreach (var hit in hits)
        {
            ISkillUser target = hit.GetComponent<ISkillUser>();
            if (target != null)
            {
                Hit(target);
                Debug.Log($"💥 {target.GetName()}에게 스플래시 피해 {damage}!");
            }
        }

        if (impactEffect != null)
        {
            GameObject fx = Instantiate(impactEffect, transform.position, Quaternion.identity);
            fx.transform.localScale = Vector3.one * splashRadius * 2f;
            Destroy(fx, 0.5f);
        }
        
        PoolManager.Instance.Release(this.GetComponent<Poolable>());
    }
}