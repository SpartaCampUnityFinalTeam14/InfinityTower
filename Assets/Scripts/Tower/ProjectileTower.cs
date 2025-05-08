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

            GameObject projObj = Instantiate(projectileDataSO.prefab, firePoint.position, Quaternion.identity);

            Projectile proj = projObj.GetComponent<Projectile>();
            if (proj == null)
            {
                Debug.LogWarning($"🚨 {projectileData.id} 프리팹에 Projectile 스크립트가 없음");
                continue;
            }

            // 여기! valueList[0] = 데미지
            float projectileDamage = (towerData.statTypes.Contains((int)StatType.attackDamage)) ? towerData.GetStatValue(StatType.attackDamage) : 0f;
            
            proj.Init(projectileData, projectileDataSO, projectileDamage);  // ⚡ 데미지 넘겨줌
            proj.SetTarget(target.transform);

            Debug.Log($"📌 SetTarget 호출됨 → {target.name}");
        }
    }

}