using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    protected float speed;
    protected float damage;
    protected GameObject impactEffect;
    protected BaseTower towerInfo;

    protected virtual void Update()
    {
        Move();
    }

    protected abstract void Move();
    
    protected void FlipByDirection(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    protected void Hit(ISkillUser target)
    {
        target.TakeDamage(damage);
        OnHitTarget((target as MonoBehaviour)?.gameObject);
        Destroy(gameObject);
    }

    protected virtual void OnHitTarget(GameObject target)
    {
        towerInfo.ApplyEffectOnAttack(target);
    }

    public virtual void SetTarget(Transform t) { }

    // ✨ 수정된 Init
    public virtual void Init(ProjectileData data, ProjectileDataSO visual, float customDamage, BaseTower towerInfo)
    {
        speed = data.speed;
        damage = customDamage; // << 타워에서 넘겨주는 데미지 사용
        impactEffect = visual.impactEffect;
        this.towerInfo = towerInfo;
        Debug.Log($"{speed}, {damage}");
    }
}