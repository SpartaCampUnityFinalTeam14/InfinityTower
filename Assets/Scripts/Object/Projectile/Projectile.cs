using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    protected float speed;
    protected float damage;
    protected GameObject impactEffect;

    protected virtual void Update()
    {
        Move();
    }

    protected abstract void Move();
    
    protected void FlipByDirection(Vector3 direction)
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (direction.x < 0 ? -1 : 1); // 왼쪽일 땐 -1, 오른쪽일 땐 +1
        transform.localScale = scale;
    }

    protected void Hit(ISkillUser target)
    {
        target.TakeDamage(damage);
        OnHitTarget(target);
        Destroy(gameObject);
    }

    protected virtual void OnHitTarget(ISkillUser target) { }

    public virtual void SetTarget(Transform t) { }

    // ✨ 수정된 Init
    public virtual void Init(ProjectileData data, ProjectileDataSO visual, float customDamage)
    {
        speed = data.speed;
        damage = customDamage; // << 타워에서 넘겨주는 데미지 사용
        impactEffect = visual.impactEffect;
    }
}