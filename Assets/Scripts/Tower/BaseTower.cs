using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.EventSystems;


public abstract class BaseTower : Poolable
{
    public int ID;
    protected TowerData towerData;
    protected float attackTimer;
    // 타워가 가지고 있는 이펙트
    protected List<EffectBase> myEffect;

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

    protected virtual void Awake()
    {
        rangePrefab = Resources.Load<GameObject>("Prefabs/Tower/RangeIndicator");
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        myEffect = towerData.ReturnEffectList();
        nowEffectedDict = new Dictionary<int, int>();
        AddModifierStat = new Dictionary<int, float>();

        attackTimer = GetFinalStatValue(StatType.attackSpeed);

        // Ability Event 등록
        StageManager.Instance.abilityManager.AbilityHandler.ResisterAddAbilityEvent("tower", AddAbilityStat);
        StageManager.Instance.abilityManager.AbilityHandler.ResisterRemoveAbilityEvent("tower", RemoveAbilityStat);
        //StageManager.Instance.abilityManager.OnAddTowerAbility += AddAbilityStat;
        //StageManager.Instance.abilityManager.OnRemoveTowerAbility += RemoveAbilityStat;
    }

    protected virtual void Update()
    {
        ShowTowerInfo();

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            Activate();
            attackTimer = towerData.GetStatValue(StatType.attackSpeed);
        }
    }

    // 효과 적용 후 종합 수치
    public float GetFinalStatValue(StatType statType)
    {
        if (statType == StatType.targetCount)
        {
            return towerData.GetStatValue(statType) + GetAddModifierValue(statType);
        }
        else
        {
            return towerData.GetStatValue(statType) * (1 + GetAddModifierValue(statType));
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

    protected abstract void Activate(); //실제행동은 하위 클래스에서 정의

    public void UpgradeTower()
    {
        Debug.Log("타워 업그레이드");
        // 타워 레벨업 로직
    }

    public void RemoveTower()
    {
        Debug.Log("<UNK> remove Tower <UNK>");
        StageManager.Instance.GetCost((int)towerData.GetStatValue(StatType.cost));
        PoolManager.Instance.Release(this);
    }

    public void CloseTowerInfo()
    {
        PoolManager.Instance.Release(rangeIndicator);
        StageManager.Instance.timeScaleManager.PopTimeScale();
    }

    public void SetCellPos(Vector3Int cellPos)
    {
        this.cellPos = cellPos;
    }

    void ShowTowerInfo()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject.GetComponent<BaseTower>().Equals(this))
            {
                var ui = UIManager.Instance.ShowUI<UITowerInfo>();
                ui.Init(transform, towerData);

                // 사거리표시
                rangeIndicator = PoolManager.Instance.Get(rangePrefab, 1, transform).GetComponent<RangeIndicator>();
                rangeIndicator.Init(towerData.GetStatValue(StatType.attackRange));

                StageManager.Instance.timeScaleManager.PushTimeScale(0.2f);
            }
        }
    }
    
    private void InitAbilityStat()
    {
        var manager = StageManager.Instance.abilityManager;

        foreach (Ability ability in manager.CurAbilities.Values)
        {
            AddAbilityStat(ability.Data);
        }
    }

    private void AddAbilityStat(AbilityData data)
    {
        if (data.targetID.Equals(-1) || towerData.id.Equals(data.targetID))
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

    private void RemoveAbilityStat(AbilityData data)
    {
        if (data.targetID.Equals(-1) || towerData.id.Equals(data.targetID))
        {
            for (int i = 0; i < data.valueType.Count; i++)
            {
                if (AddModifierStat.ContainsKey(data.valueType[i]))
                {
                    AddModifierStat[data.valueType[i]] -= DataManager.Instance.abilityDict[data.perkID].value[i];
                }
            }
        }
    }
}
