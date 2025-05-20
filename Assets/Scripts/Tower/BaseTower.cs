using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseTower : Poolable
{
    public int ID;
    protected TowerData towerData;
    protected float attackTimer;
    // key : effectID / value : 타워가 가지고 있는 이펙트
    protected Dictionary<int,EffectBase> myEffectDict;

    // <key : 받는 이펙트의 statusID / value: 현재 적용된 이펙트 카운트> 본인이 받고있는 이펙트를 저장
    public Dictionary<int, int> nowEffectedDict;
    // 적용되는 statType의 ID 값들 , 변동되는 스탯에 대한 수치
    public Dictionary<int, float> AddModifierStat;

    protected Animator anim;
    protected SpriteRenderer spriteRenderer;

    GameObject rangePrefab;
    RangeIndicator rangeIndicator;

    // 타워가 설치된 타일 위치
    Vector3Int cellPos;
    
    protected float baseAttackSpeed = 1f; 

    // 업그레이드 관련
    //protected int currentLevel = 1;
    //protected int maxLevel = 3; // 기본 최대 레벨 (필요시 TowerData에 넣어도 좋음)

    protected virtual void Awake()
    {
        rangePrefab = Resources.Load<GameObject>("Prefabs/Tower/RangeIndicator");
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnDisable()
    {
        TilemapManager.Instance.UnregisterOccupiedCell(cellPos);
    }

    protected virtual void Start()
    {
        TowerInit();
        InitAbilityStat();
    }

    public void TowerInit()
    {
        towerData = DataManager.Instance.towerDict[ID];
        myEffectDict = towerData.ReturnEffectList();
        nowEffectedDict = new Dictionary<int, int>();
        AddModifierStat = new Dictionary<int, float>();

        InitArtifactStatModifier(AddModifierStat, IsValidStat);

        // Ability Event 등록
        //StageManager.Instance.abilityManager.AbilityHandle.ResisterAddAbilityEvent("tower", AddAbilityStat);
        //StageManager.Instance.abilityManager.AbilityHandle.ResisterRemoveAbilityEvent("tower", RemoveAbilityStat);
        //StageManager.Instance.abilityManager.OnAddTowerAbility += AddAbilityStat;
        //StageManager.Instance.abilityManager.OnRemoveTowerAbility += RemoveAbilityStat;
    }

    public void InitArtifactStatModifier(Dictionary<int, float> AddModifier, Func<StatType, bool> isValidStat)
    {
        foreach (var artifactData in SaveManager.Instance.artifactLevelDict)
        {
            ////아티펙트가 가진 TargetType을 검사
            //if (artifactData.Value.ReturnMyTargetType())
            //    continue;

            //유닛이 가지고있는 스탯 타입을 검사
            if (!isValidStat(artifactData.Value.ReturnMyStatType()))
                continue;

            int artifactStatType = (int)artifactData.Value.ReturnMyStatType();
            float value = artifactData.Value.ReturnNowStatValue((StatType)artifactStatType);
            if (!AddModifier.TryAdd(artifactStatType, value))
            {
                AddModifier[artifactStatType] += value;
            }
        }
    }

    private bool IsValidStat(StatType statType)
    {
        foreach (int type in towerData.statType)
        {
            if(type == (int)statType)
            {
                return true;
            }
        }
        return false;
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
        //if (currentLevel >= maxLevel)
        //{
        //    Debug.Log("최대 레벨입니다.");
        //    return;
        //}

        //int upgradeID = ID + 1;
        //if (!DataManager.Instance.towerDict.ContainsKey(upgradeID))
        //{
        //    Debug.Log("업그레이드 데이터가 존재하지 않습니다.");
        //    return;
        //}

        //int cost = (int)towerData.GetStatValue(StatType.cost);
        //if (!StageManager.Instance.UseCost(cost))
        //{
        //    Debug.Log("코스트 부족");
        //    return;
        //}

        //currentLevel++;
        //ID = upgradeID;
        //towerData = DataManager.Instance.towerDict[ID];

        //myEffectDict = towerData.ReturnEffectList();
        //attackTimer = GetFinalStatValue(StatType.attackSpeed);

        //AddModifierStat.Clear();
        //InitAbilityStat();

        //Debug.Log($"타워가 레벨 {currentLevel}로 업그레이드 되었습니다.");
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
        var manager = StageManager.Instance.abilityManager;

        //foreach (Ability ability in manager.allAbilities.Values)
        //{
        //    AddAbilityStat(ability.Data);
        //}

        // Update
        if (manager.AbilityHandle.TryGetAbilities((int)TargetType.Tower, out var list))
        {
            foreach (Ability ability in list)
            {
                AddAbilityStat(ability.Data);
            }
        }
    }

    private void AddAbilityStat(AbilityData data)
    {
        if (!gameObject.activeSelf)
            return;

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

    //private void RemoveAbilityStat(AbilityData data)
    //{
    //    if (data.targetID.Equals(-1) || towerData.id.Equals(data.targetID))
    //    {
    //        for (int i = 0; i < data.valueType.Count; i++)
    //        {
    //            if (AddModifierStat.ContainsKey(data.valueType[i]))
    //            {
    //                AddModifierStat[data.valueType[i]] -= DataManager.Instance.abilityDict[data.perkID].value[i];
    //            }
    //        }
    //    }
    //}
    
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
