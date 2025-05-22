using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;


public abstract class BaseTower : Poolable, IBuffable
{
    public int ID;
    protected TowerData towerData;
    protected float attackTimer;
    // key : effectID / value : 타워가 가지고 있는 이펙트
    protected Dictionary<int,EffectBase> myEffectDict;

    // <key : 받는 이펙트의 statusID / value: 현재 적용된 이펙트 카운트> 본인이 받고있는 이펙트를 저장
    public Dictionary<int, int> nowEffectedDict;
    // 적용되는 statType의 ID 값들 , 변동되는 스탯에 대한 수치
    public Dictionary<int, float> AddModifierStat { get; set; }
    
    // 해당 타워가 영향을 받는 스탯 타입들
    public List<int> ValidStatTypes => towerData.statType;

    protected Animator anim;
    protected SpriteRenderer spriteRenderer;

    GameObject rangePrefab;
    RangeIndicator rangeIndicator;

    // 타워가 설치된 타일 위치
    Vector3Int cellPos;
    
    protected float baseAttackSpeed = 1f; 

    protected virtual void Awake()
    {
        rangePrefab = Resources.Load<GameObject>("Prefabs/Tower/RangeIndicator");
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnEnable()
    {
        if (AddModifierStat != null)
            InitAbilityStat();
    }

    private void OnDisable()
    {
        RemoveAbilityStat();

        TilemapManager.Instance.UnregisterOccupiedCell(cellPos);
    }

    protected virtual void Start()
    {
        TowerInit();
    }

    public void TowerInit()
    {
        towerData = DataManager.Instance.towerDict[ID];
        myEffectDict = towerData.ReturnEffectList();
        nowEffectedDict = new Dictionary<int, int>();
        AddModifierStat = new Dictionary<int, float>();

        ArtifactHelper.ApplyArtifactModifiers(this);
        InitAbilityStat();
    }

    protected virtual void Update()
    {
        ShowTowerInfo();

        attackTimer = Mathf.Min(attackTimer, 1 / GetFinalStatValue(StatType.attackSpeed));
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0 && IsAttactAvailable())
        {
            Activate();
            attackTimer = 1/GetFinalStatValue(StatType.attackSpeed);
        }
    }

    protected virtual bool IsAttactAvailable() { return true; }

    // 효과 적용 후 종합 수치
    public float GetFinalStatValue(StatType statType)
    {
        if (statType == StatType.targetCount)
        {
            return towerData.GetStatValue(statType) + GetAddModifierValue(statType);
        }
        else
        {
            return Mathf.Max(0, towerData.GetStatValue(statType) * (1 + GetAddModifierValue(statType))) ;
        }
    }

    public float GetAddModifierValue(StatType type)
    {
        if (AddModifierStat.TryGetValue((int)type,out float value))
        {
            return value;
        }
        return 0f;
    }

    public void ApplyEffectOnAttack(GameObject target)
    {
        if (!target) return;

        if (towerData.TargetType == TargetType.Enemy)
        {
            MonsterBase enemy = target.GetComponent<MonsterBase>();
            foreach (var T in myEffectDict)
            {
                if (enemy == null) continue;
                // 디버프는 항상 적용
                float[] effectValues = towerData.effectValue[towerData.effectID.IndexOf(T.Key)].values;
                T.Value.ApplyEffect_Monster(enemy, effectValues[0], effectValues[1], effectValues[2] > 0);
                Debug.Log($"디버프 {(EffectType)T.Key} {effectValues[0]} 적용 (지속: {effectValues[1]}) -> {enemy.name}");
            }
        }
        else if (towerData.TargetType == TargetType.Tower)
        {
            // 아군 버프
            TargettingTower ally = target.GetComponent<TargettingTower>();
            foreach (var T in myEffectDict)
            {
                if (ally == null) continue;
                float[] effectValues = towerData.effectValue[towerData.effectID.IndexOf(T.Key)].values;
                T.Value.ApplyEffect_Tower(ally, effectValues[0], effectValues[1], effectValues[2] > 0);
                Debug.Log($"버프 {(EffectType)T.Key} {effectValues[0]} 적용 (지속: {effectValues[1]}) -> {ally.name}");
            }
        }
    }

    protected abstract void Activate(); //실제행동은 하위 클래스에서 정의

    public void UpgradeTower()
    {
        Debug.Log("타워 업그레이드");
        // 타워 레벨업 로직
    }

    public void RemoveTower()
    {
        StageManager.Instance.GetCost((int)towerData.GetStatValue(StatType.cost));
        PoolManager.Instance.Release(this);
    }

    public void CloseTowerInfo()
    {
        PoolManager.Instance.Release(rangeIndicator);
        //StageManager.Instance.timeScaleManager.PopTimeScale();
    }

    public void SetCellPos(Vector3Int cellPos)
    {
        this.cellPos = cellPos;
    }

    void ShowTowerInfo()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //if (EventSystem.current.IsPointerOverGameObject())
            if (Util.IsPointerOverUIObject())
                return;

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            
            if (hit.collider != null && hit.collider.gameObject.GetComponent<BaseTower>().Equals(this))
            {
                var ui = UIManager.Instance.ShowUI<UITowerInfo>();
                ui.Init(transform, towerData);

                // 사거리표시
                rangeIndicator = PoolManager.Instance.Get(rangePrefab, 1, transform).GetComponent<RangeIndicator>();
                rangeIndicator.Init(GetFinalStatValue(StatType.attackRange));

                //StageManager.Instance.timeScaleManager.PushTimeScale(0.2f);
            }
        }
    }
    
    private void InitAbilityStat()
    {
        var list = StageManager.Instance.abilityManager.GetAbilities((int)TargetType.Tower);
        
        foreach (Ability ability in list)
        {
            AddAbilityStat(ability.Data);
        }
    }

    private void AddAbilityStat(AbilityData data)
    {
        if (data.targetID.Count <= 0 || data.targetID.Contains(towerData.id))
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
        var list = StageManager.Instance.abilityManager.GetAbilities((int)TargetType.Tower);

        foreach (Ability ability in list)
        {
            if (ability.Data.targetID.Count <= 0 || ability.Data.targetID.Contains(towerData.id))
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

    public void PlayAttackSFX()
    {
        if (ID >= 0 && ID <= 5)
        {
            SoundManager.Instance.PlaySFX(SFX.Attack_Soldier);
        }
        else if (ID == 12 || ID == 13)
        {
            SoundManager.Instance.PlaySFX(SFX.Attack_Wizard);
        }
        else if (ID == 14)
        {
            SoundManager.Instance.PlaySFX(SFX.Attack_MagicProfessor);
        }
        else
        {
            SoundManager.Instance.PlaySFX(SFX.Attack_Soldier);
        }
    }

    protected void PlayAttackAnimation(Vector3 targetPos)
    {
        anim?.SetTrigger("Attack");

        // 방향 설정
        Vector2 dir = (targetPos - transform.position).normalized;
        if (spriteRenderer)
            spriteRenderer.flipX = dir.x < 0;
    }
    
    protected void SetAttackAnimationSpeed()
    {
        float currentAttackSpeed = GetFinalStatValue(StatType.attackSpeed);

        anim.speed = currentAttackSpeed <= baseAttackSpeed 
            ? baseAttackSpeed 
            : currentAttackSpeed;
    }
}
