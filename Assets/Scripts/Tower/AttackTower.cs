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
                bulletScript.SetDamage(towerData.GetValue(BuffEffectType.AttackPower));
            }
        }
    }
}