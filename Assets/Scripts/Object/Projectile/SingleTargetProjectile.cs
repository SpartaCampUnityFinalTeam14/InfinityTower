using UnityEngine;

public class SingleTargetProjectile : Projectile
{
    private Transform target;
    private Vector3 lastKnownPosition;
    private bool targetLost = false;

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

        // 타겟이 살아있거나, 죽었지만 위치 추적을 끝까지 하도록 이동
        Vector3 dir = (lastKnownPosition - transform.position).normalized;

        FlipByDirection(dir);
        
        Vector3 moveDir = dir * speed * Time.deltaTime;

        if (Vector3.SqrMagnitude(transform.position - lastKnownPosition) < moveDir.sqrMagnitude)
        {
            if (!targetLost)
            {
                ISkillUser hit = target.GetComponent<ISkillUser>();
                if (hit != null)
                    Hit(hit);
                PoolManager.Instance.Release(this.GetComponent<Poolable>());
            }
            else
            {
                Destroy(gameObject); // 💥 그냥 날아가다가 사라지게
            }
        }
        else transform.position += moveDir;
    }
}