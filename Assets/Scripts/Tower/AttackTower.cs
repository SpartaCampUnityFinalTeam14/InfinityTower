using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTower : TargettingTower
{
    public GameObject bulletPrefab; // 발사할 총알 프리팹
    public Transform firePoint;     // 총알이 발사될 위치

    protected override void UseActOnTargets()
    {
        foreach (GameObject target in targets)
        {
            if (target == null) continue;

            // 총알을 발사
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetTarget(target.transform);
                bulletScript.SetDamage(towerData.value);
            }
        }
    }
}

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
            //target.GetComponent<MonsterBase>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
