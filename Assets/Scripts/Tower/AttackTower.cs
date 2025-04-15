using UnityEngine;

public class SOAttackTower : TargettingTower
{
    public ProjectileDataSO projectileData;
    public Transform firePoint;

    protected override void UseActOnTargets()
    {
        foreach (GameObject target in targets)
        {
            if (target == null || projectileData == null)
            {
                Debug.LogWarning("🚨 타겟 또는 프로젝트일 데이터가 null임");
                continue;
            }

            Debug.Log($"🧨 타겟에게 발사 중: {target.name}");

            GameObject projObj = Instantiate(projectileData.prefab, firePoint.position, Quaternion.identity);

            Projectile proj = projObj.GetComponent<Projectile>();
            if (proj == null)
            {
                Debug.LogWarning($"🚨 {projectileData.id} 프리팹에 Projectile 스크립트가 없음");
                continue;
            }

            proj.Init(projectileData);
            proj.SetTarget(target.transform);
            Debug.Log($"📌 SetTarget 호출됨 → {target.name}");
        }
    }
}