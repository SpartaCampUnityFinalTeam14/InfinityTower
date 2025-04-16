using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    protected float speed;
    protected float damage;
    protected GameObject impactEffect;

    public virtual void Init(float speed, float damage)
    {
        this.speed = speed;
        this.damage = damage;
    }

    protected virtual void Update()
    {
        Move();
    }

    protected abstract void Move();

    protected void Hit(ISkillUser target)
    {
        target.TakeDamage(damage);
        OnHitTarget(target); 
        Destroy(gameObject);
    }
    protected virtual void OnHitTarget(ISkillUser target) { }
    
    public virtual void SetTarget(Transform t) { }
    
    public virtual void Init(ProjectileDataSO data)
    {
        speed = data.speed;
        damage = data.damage;
        impactEffect = data.impactEffect;
    }

}