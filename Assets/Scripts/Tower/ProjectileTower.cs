using UnityEngine;

public class ProjectileTower : TargettingTower
{
    public ProjectileDataSO projectileDataSO;
    public Transform firePoint;
    private ProjectileData projectileData;

    protected override void Awake()
    {
        base.Awake();

        if (DataManager.Instance.projectileDataDict.TryGetValue(projectileDataSO.id, out var data))
        {
            projectileData = data;
        }
        else
        {
            Debug.LogWarning($"🚨 ProjectileData ID {projectileData.id}를 찾을 수 없습니다.");
        }
    }
    
    protected override void Update()
    {
        base.Update();
    }

    protected override void UseActOnTargets()
    {
        foreach (GameObject target in targets)
        {
            if (target == null || projectileDataSO == null)
            {
                Debug.LogWarning("🚨 타겟 또는 프로젝트일 데이터가 null임");
                continue;
            }

            Debug.Log($"🧨 타겟에게 발사 중: {target.name}");

            Poolable pooled = PoolManager.Instance.Get(projectileDataSO.prefab, 10, null);
            GameObject projObj = pooled.gameObject;

            projObj.transform.position = firePoint.position;
            projObj.transform.rotation = Quaternion.identity;

            Projectile proj = projObj.GetComponent<Projectile>();
            if (proj == null)
            {
                Debug.LogWarning($"🚨 {projectileData.id} 프리팹에 Projectile 스크립트가 없음");
                continue;
            }

            // valueList[0] = 데미지
            float projectileDamage = (towerData.statType.Contains((int)StatType.attackDamage)) 
                ? GetFinalStatValue(StatType.attackDamage) 
                : 0f;

            proj.Init(projectileData, projectileDataSO, projectileDamage, this);
            proj.SetTarget(target.transform);

            Debug.Log($"📌 SetTarget 호출됨 → {target.name}");
        }
    }

}