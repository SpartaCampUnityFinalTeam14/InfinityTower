using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    private float speed = 10f;
    private int damage;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 dir = (target.position - transform.position).normalized;
        transform.Translate(dir * speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.2f)
        {
            target.GetComponent<MonsterBase>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
