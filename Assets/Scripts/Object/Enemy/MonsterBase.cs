using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MonsterBase : Poolable, ISkillUser
{
    private Floor floor;
    protected MonsterData data;

    List<Vector3> pathPoints;
    int curTileIdx = 0;
    public int currentHP;
    //방어력 추가
    private float defense;
    // 이동속도 
    private float moveSpeed;
    private bool isDead;
    public bool IsDead => isDead;

    private Image hpBar;

    public virtual void Init(int id, List<Vector3> path, Transform startPos, Floor floor)
    {
        this.floor = floor;
        isDead = false;
        data = new(DataManager.Instance.monsterDict[id]);

        currentHP = (int)GetStat(StatType.HP);
        moveSpeed = GetStat(StatType.moveSpeed);
        defense = GetStat(StatType.armor);

        ApplyTypeBonus((EnemyType)data.enemyType);

        transform.position = startPos.position;
        SetPath(path);

        // ✅ HP바 연결
        hpBar = transform.Find("HPBar/Image").GetComponent<Image>();
        UpdateHpUI(); // 시작할 때 체력바도 세팅
    }

    public void UpdateHpUI()
    {
        if (hpBar != null)
        {
            hpBar.fillAmount = Mathf.Clamp01((float)currentHP / GetStat(StatType.HP));
        }
    }
    
    public void TakeDamage(float amount)
    {
        if (isDead)
            return;

        currentHP -= Mathf.RoundToInt(amount);

        UpdateHpUI(); // ✅ 체력 갱신

        if (currentHP <= 0)
            Dead();
    }

    public void SetPath(List<Vector3> path)
    {
        pathPoints = path;
        curTileIdx = 0;

        if (pathPoints != null && pathPoints.Count > 0)
        {
            transform.position = pathPoints[0]; // 시작 위치 설정
            StartCoroutine(MoveToPath());
        }
    }

    protected IEnumerator MoveToPath()
    {
        while (curTileIdx < pathPoints.Count)
        {
            Vector3 target = pathPoints[curTileIdx];

            while (Vector3.Distance(transform.position, target) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, GetStat(StatType.moveSpeed) * Time.deltaTime);
                yield return null;
            }

            transform.position = target;
            curTileIdx++;
            yield return null;
        }

        StageManager.Instance.TakeDamage((int)GetStat(StatType.damage));
        Dead();
    }

    void Dead()
    {
        if (isDead) return;

        isDead = true;

        StopAllCoroutines(); // <<<< 코루틴 싹 멈춰서 이동 정지

        Animator animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("isDead");
        }

        StartCoroutine(Co_Dead());
    }
    
    private IEnumerator Co_Dead()
    {
        // 애니메이션 길이만큼 기다리기 (예시 0.5초)
        yield return new WaitForSeconds(0.5f);

        floor.SubrtactMonsterCount(1);
        PoolManager.Instance.Release(this);
    }

    protected void ApplyTypeBonus(EnemyType type)
    {
        Debug.Log($"적의 타입 : {type}, 이동속도 : {moveSpeed}, 체력 : {currentHP}, 방어력 : {defense}");
        switch (type)
        {
            case EnemyType.Fast:
                moveSpeed *= 2f; // 빠른 몬스터는 이동속도 1.5배
                break;
        
            case EnemyType.Tank:
                currentHP = Mathf.RoundToInt(currentHP * 2f); // 탱커 몬스터는 체력 2배
                defense *= 2f; // 방어력도 보너스
                break;
            case EnemyType.Boss:
                Debug.Log($"보스몬스터, {data.hasSkill} ");
                break;
            case EnemyType.Normal:
            default:
                break;
        }
        Debug.Log($"적의 타입 : {type}, 이동속도 : {moveSpeed}, 체력 : {currentHP}, 방어력 : {defense}");
    }
    
    public float GetStat(StatType type)
    {
        int iType = (int)type;
        var common = StageManager.Instance.abilityManager.monsterAbilities;

        float origin = 0f;
        float abil = 0f;

        bool result = data.dictValue.TryGetValue(iType, out origin);
        abil = common.ContainsKey(iType) ? common[iType] : 0f;
        
        Debug.Assert(result, $"Not Find Type in DictionaryValue");
        
        return origin + abil;
    }
    
    public string GetName()
    {
        return data.name;
    }
    
    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
