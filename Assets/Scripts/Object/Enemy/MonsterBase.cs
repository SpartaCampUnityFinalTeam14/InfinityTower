using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MonsterBase : Poolable, ISkillUser, IBuffable
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

    // <key : 받는 이펙트의 statusID / value: 현재 적용된 이펙트 카운트> 본인이 받고있는 이펙트를 저장
    public Dictionary<int, int> nowEffectedDict;

    // 적용되는 statType의 ID 값들 , 변동되는 스탯에 대한 수치
    public Dictionary<int, float> AddModifierStat { get; set; }

    // 해당 몬스터가 영향을 받는 스탯 타입들
    public List<int> ValidStatTypes => data.valueType;

    [SerializeField] private MonsterSpriteSetSO spriteSet;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Sprite[] currentAnimation = null;
    private int currentFrame = 0;
    private float frameTimer = 0f;
    private float frameRate = 0.15f;
    
    protected List<Skill> skills = new();

    private void OnEnable()
    {
        if (AddModifierStat != null)
            InitAbilityStat();
    }

    private void OnDisable()
    {
        RemoveAbilityStat();
    }

    public virtual void Init(int id, List<Vector3> path, Transform startPos, Floor floor)
    {
        this.floor = floor;
        isDead = false;
        data = new(DataManager.Instance.monsterDict[id]);

        currentHP = (int)GetFinalStatValue(StatType.HP);
        moveSpeed = GetFinalStatValue(StatType.moveSpeed);

        defense = GetFinalStatValue(StatType.armor);

        nowEffectedDict = new Dictionary<int, int>();
        AddModifierStat = new Dictionary<int, float>();

        ArtifactHelper.ApplyArtifactModifiers(this);
        InitAbilityStat();

        ApplyTypeBonus((EnemyType)data.enemyType);

        transform.position = startPos.position;
        SetPath(path);

        // ✅ HP바 연결
        hpBar = transform.Find("HPBar/Image")?.GetComponent<Image>();
        if (hpBar == null)
            Debug.LogWarning("❌ HPBar 연결 실패! 경로 확인 필요.");

        UpdateHpUI(); // 시작할 때 체력바도 세팅
    }

    public void UpdateHpUI()
    {
        if (this == null || gameObject == null || hpBar == null || !hpBar.gameObject.activeInHierarchy)
            return;

        hpBar.fillAmount = Mathf.Clamp01((float)currentHP / GetFinalStatValue(StatType.HP));
    }
    
    public void TakeDamage(float amount)
    {
        if (isDead)
            return;

        SoundManager.Instance.PlaySFX(SFX.Hit_Monster);
        currentHP -= Mathf.RoundToInt(amount);
        UpdateHpUI();
        
        ShowDamagePopup(Mathf.RoundToInt(amount));

        if (currentHP <= 0)
        {
            Dead();
        }
    }
    private void ShowDamagePopup(int damage)
    {
        GameObject popupPrefab = Resources.Load<GameObject>("Prefabs/UI/UI_DamagePopup");
        if (popupPrefab == null)
        {
            Debug.LogWarning("❌ DamagePopup 프리팹을 불러올 수 없습니다.");
            return;
        }

        Vector3 popupPos = transform.position + new Vector3(0f, 0.5f, 0f); // 몬스터 위쪽
        GameObject popupGO = Instantiate(popupPrefab, popupPos, Quaternion.identity);
        DamagePopup popup = popupGO.GetComponent<DamagePopup>();
        popup.Setup(damage);
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
                    Skill newSkill = SkillFactory.CreateSkill(skillData, data.value[3]);
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
            return data.GetStat(statType) + GetAddModifierValue(statType);
        }
        else if (statType == StatType.HP)
        {
            //Debug.Log("레벨스케일링 적용");
            return data.GetStat(statType) * (1 + GetAddModifierValue(statType)) * StageManager.Instance.monsterLevelScaling;
        }
        else
        {
            return data.GetStat(statType) * (1 + GetAddModifierValue(statType));
        }
    }

    public float GetAddModifierValue(StatType type)
    {
        if (AddModifierStat==null)
        {
            return 0f;
        }
        if (AddModifierStat.TryGetValue((int)type, out float value))
        {
            return value;
        }
        return 0f;
    }

    private void InitAbilityStat()
    {
        var list = StageManager.Instance.abilityManager.GetAbilities((int)TargetType.Enemy);

        foreach (Ability ability in list)
        {
            AddAbilityStat(ability.Data);
        }
    }

    private void AddAbilityStat(AbilityData data)
    {
        if (data.targetID.Count <= 0 || data.targetID.Contains(this.data.id))
        {
            for (int i = 0; i < data.valueType.Count; i++)
            {
                if (!AddModifierStat.TryAdd(data.valueType[i], data.value[i]))
                {
                    AddModifierStat[data.valueType[i]] += data.value[i];
                }
            }
        }
    }

    private void RemoveAbilityStat()
    {
        var list = StageManager.Instance.abilityManager.GetAbilities((int)TargetType.Enemy);

        foreach (Ability ability in list)
        {
            if (ability.Data.targetID.Count <= 0 || ability.Data.targetID.Contains(this.data.id))
            {
                for (int i = 0; i < ability.Data.valueType.Count; i++)
                {
                    if (AddModifierStat.TryGetValue(ability.Data.valueType[i], out float value))
                    {
                        value -= ability.Data.value[i];
                        MathF.Max(value, 0f);
                    }
                }
            }
        }

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
    
    public float GetBaseDamage()
    {
        return 10f; // 특성, 유물 등 계산 가능
    }

}
