using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField]private float moveSpeed;
    private bool isDead;
    public bool IsDead => isDead;

    private Image hpBar;

    // <key : 받는 이펙트의 statusID / value: 현재 적용된 이펙트 카운트> 본인이 받고있는 이펙트를 저장
    public Dictionary<int, int> nowEffectedDict = new();
    // 적용되는 statType의 ID 값들 , 변동되는 스탯에 대한 수치
    public Dictionary<int, float> AddModifierStat = new();
    
    [SerializeField] private MonsterSpriteSetSO spriteSet;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Sprite[] currentAnimation = null;
    private int currentFrame = 0;
    private float frameTimer = 0f;
    private float frameRate = 0.15f;
    
    protected List<Skill> skills = new();

    public virtual void Init(int id, List<Vector3> path, Transform startPos, Floor floor)
    {
        this.floor = floor;
        isDead = false;
        data = new(DataManager.Instance.monsterDict[id]);

        currentHP = (int)GetFinalStatValue(StatType.HP);
        moveSpeed = GetFinalStatValue(StatType.moveSpeed);
        defense = GetFinalStatValue(StatType.armor);

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
            hpBar.fillAmount = Mathf.Clamp01((float)currentHP / GetFinalStatValue(StatType.HP));
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
                Vector3 dir = (target - transform.position).normalized;

                // ✅ 이동
                transform.position = Vector3.MoveTowards(transform.position, target, GetFinalStatValue(StatType.moveSpeed) * Time.deltaTime);
                moveSpeed = GetFinalStatValue(StatType.moveSpeed);

                // ✅ 방향에 따른 스프라이트 업데이트
                UpdateDirectionSprite(dir);

                yield return null;
            }

            transform.position = target;
            curTileIdx++;
            yield return null;
        }

        StageManager.Instance.TakeDamage((int)GetFinalStatValue(StatType.damage));
        Dead();
    }
    
    private void AnimateWalk(Sprite[] walkSprites)
    {
        if (walkSprites == null || walkSprites.Length == 0)
        {
            Debug.LogWarning("🛑 걷기 스프라이트 배열이 비어있음!");
            return;
        }

        if (currentAnimation != walkSprites)
        {
            currentAnimation = walkSprites;
            currentFrame = 0;
            frameTimer = 0f;
        }

        frameTimer += Time.deltaTime;
        if (frameTimer >= frameRate)
        {
            currentFrame = (currentFrame + 1) % walkSprites.Length;
            spriteRenderer.sprite = currentAnimation[currentFrame];
            frameTimer = 0f;
        }
    }

    private void UpdateDirectionSprite(Vector3 dir)
    {
        if (spriteRenderer == null || spriteSet == null) return;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            AnimateWalk(dir.x > 0 ? spriteSet.walkRight : spriteSet.walkLeft);    
        else
            AnimateWalk(dir.y > 0 ? spriteSet.walkUp : spriteSet.walkDown);   
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
        var deathSprites = spriteSet.death;
        for (int i = 0; i < deathSprites.Length; i++)
        {
            spriteRenderer.sprite = deathSprites[i];
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.2f);

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
                BossSkill();
                break;
            case EnemyType.Normal:
            default:
                break;
        }
        Debug.Log($"적의 타입 : {type}, 이동속도 : {moveSpeed}, 체력 : {currentHP}, 방어력 : {defense}");
    }

    protected void BossSkill()
    {
        if (data.hasSkill && data.skillIds != null)
        {
            foreach (var skillId in data.skillIds)
            {
                if (DataManager.Instance.skillDict.TryGetValue(skillId, out var skillData))
                {
                    Skill newSkill = SkillFactory.CreateSkill(skillData);
                    if (newSkill != null)
                    {
                        skills.Add(newSkill);
                        Debug.Log($"⚡ {data.name} 스킬 추가됨: {newSkill.skillName}");
                    }
                }
                else
                {
                    Debug.LogWarning($"⚠️ 스킬 ID {skillId}는 SkillDict에 존재하지 않음");
                }
            }

            if (skills.Count > 0)
            {
                Debug.Log($"🔥 {data.name} 의 스킬 목록: {string.Join(", ", skills.Select(s => s.skillName))}");
                StartCoroutine(SkillRoutine());
            }
        }
    }

    private IEnumerator SkillRoutine()
    {
        while (!isDead)
        {
            foreach (Skill skill in skills)
            {
                if (skill is ActiveSkill activeSkill && activeSkill.CanUse())
                {
                    if (skill is TargetPositionSkill tp)
                        tp.TryStartSkill(this, transform.position);
                    else
                        activeSkill.Trigger(this);
                }
            }
            yield return new WaitForSeconds(10f);
        }
    }

    // 효과 적용 후 종합 수치
    public float GetFinalStatValue(StatType statType)
    {
        if (statType == StatType.targetCount)
        {
            return GetStat(statType) + GetAddModifierValue(statType);
        }
        else
        {
            return GetStat(statType) * (1 + GetAddModifierValue(statType));
        }
    }

    public float GetAddModifierValue(StatType type)
    {
        if (AddModifierStat.TryGetValue((int)type, out float value))
        {
            return value;
        }
        return 0f;
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
    
    public int GetTeam()
    {
        return 0; // 몬스터는 팀 0
    }

}
