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
    private float moveSpeed;
    private bool isDead;
    public bool IsDead => isDead;

    private Image hpBar;

    // 디버프 관련 상태 저장
    private Dictionary<EffectType, Coroutine> debuffCoroutines = new Dictionary<EffectType, Coroutine>();
    private float originalMoveSpeed;
    private float originalDefense;
    
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

        currentHP = (int)GetStat(StatType.Health);
        moveSpeed = GetStat(StatType.Speed);
        defense = GetStat(StatType.Armor);

        ApplyTypeBonus((EnemyType)data.enemyType);

        transform.position = startPos.position;
        SetPath(path);
        
        //디버프 해제 후 원상복구를 위한 저장
        originalMoveSpeed = moveSpeed;
        originalDefense = defense/* = data.defense*/; //몬스터 데이터에 방어력 추가 시 주석 해제

        // ✅ HP바 연결
        hpBar = transform.Find("HPBar/Image").GetComponent<Image>();
        UpdateHpUI(); // 시작할 때 체력바도 세팅
    }

    public void UpdateHpUI()
    {
        if (hpBar != null)
        {
            hpBar.fillAmount = Mathf.Clamp01((float)currentHP / GetStat(StatType.Health));
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
                transform.position = Vector3.MoveTowards(transform.position, target, GetStat(StatType.Speed) * Time.deltaTime);

                // ✅ 방향에 따른 스프라이트 업데이트
                UpdateDirectionSprite(dir);

                yield return null;
            }

            transform.position = target;
            curTileIdx++;
            yield return null;
        }

        StageManager.Instance.TakeDamage((int)GetStat(StatType.Attack));
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

    
    //디버프 적용메서드
    public void ApplyDebuff(EffectType type, float amount, float duration)
    {
        // 기존 디버프가 있으면 정지
        if (debuffCoroutines.TryGetValue(type, out Coroutine running))
        {
            StopCoroutine(running);
        }

        // 새로운 디버프 적용
        Coroutine routine = StartCoroutine(DebuffRoutine(type, amount, duration));
        debuffCoroutines[type] = routine;
    }

    private IEnumerator DebuffRoutine(EffectType type, float amount, float duration)
    {
        switch (type)
        {
            case EffectType.Slow:
                moveSpeed = Mathf.Max(0.1f, originalMoveSpeed - amount);
                break;

            case EffectType.DefenseDown:
                defense = Mathf.Max(0, originalDefense - amount);
                break;
        }

        yield return new WaitForSeconds(duration);

        // 원래 값으로 복원
        switch (type)
        {
            case EffectType.Slow:
                moveSpeed = originalMoveSpeed;
                break;

            case EffectType.DefenseDown:
                defense = originalDefense;
                break;
        }

        // 디버프 딕셔너리에서 제거
        debuffCoroutines.Remove(type);
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
