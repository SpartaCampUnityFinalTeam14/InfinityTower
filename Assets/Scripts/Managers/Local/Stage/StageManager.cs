using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    private int maxHp;
    private int hp;
    private int healAdd = 1;
    private float curCost;
    public float CurrentCost => curCost;
    private float startCost = 100;

    private Dictionary<int, float> abilityMultiplier = new(); // 영웅 특성 버프 리스트 Dictionary<StatType, value>

    [HideInInspector] public int token;
    private int baseTokenAdd = 450;
    private int floorTokenAdd = 100;
    [HideInInspector] public int book;

    //private float maxCost = 10f;
    [SerializeField] private float costRecoveryMultiplier = 18f; // Cost 얻는 속도 - 기본 1배속
    private List<float> activeCostRecoveryMultipliers = new List<float>(); // 여러 타워의 버프들을 저장

    [HideInInspector] public List<int> selectedTowers;
    [HideInInspector] public int selectedChampion;

    [HideInInspector] public AbilityManager abilityManager;
    [HideInInspector] public EventManager eventManager;
    [HideInInspector] public TimeScaleManager timeScaleManager;

    [HideInInspector] public SkillTargetingSystem skillTargetingSystem;
    [HideInInspector] public SkillVisualDB skillVisualDB;
    public Hero CurHero { get; set; }

    private int stageIndex;
    private int floorId;
    private int maxFloor;
    private int floorCount = 0;
    private GameObject floorGO;
    private Floor curFloor;
    public Floor CurFloor => curFloor;
    private Coroutine stageCoroutine;
    private float timer;

    [SerializeField] private FloatEventChannel OnCostChanged;
    [SerializeField] private IntEventChannel OnPlayerHpChanged;
    [SerializeField] private IntEventChannel OnFloorCountChanged;
    [SerializeField] private EventChannel OnResetTowerCoolDown;

    [HideInInspector] public bool isEventEnd;
    [HideInInspector] public bool isIntroEnd;
    [HideInInspector] public bool isAdditionalFloor;
    [HideInInspector] public bool isFadeComplete;

    public float monsterLevelScaling;

    protected override void Awake()
    {
        isGlobal = false;

        base.Awake();

        //영웅 스킬 세팅 필요
        selectedTowers = new(SaveManager.Instance.playerData.selectedTowerIndex);
        selectedChampion = SaveManager.Instance.playerData.selectedChampionIndex;
        maxHp = DataManager.Instance.championDict[selectedChampion].hp;
        hp = maxHp;
        CurHero = new Hero();

        //abilityManager = GetComponent<AbilityManager>();
        Init();
        ApplyArtifact();
        StartStage(); //추후 awake가 아닌 다른 곳으로 이동 (예를 들어, 시작 버튼을 누른다든가 하는 식)
    }

    private void Start()
    {
        OnPlayerHpChanged.RaiseEvent(hp);
        OnFloorCountChanged.RaiseEvent(1);
        SendAnalytics("STAGE_STARTED");
    }

    private void Update()
    {
        timer += Time.deltaTime;
    }

    public void SendAnalytics(string eventName, bool playerQuit = false)
    {
        var eventData = BuildBaseAnalyticsData();

        switch (eventName)
        {
            case "STAGE_STARTED":
                AppendStageStartData(eventData);
                break;
            case "STAGE_CLEARED":
                AppendStageClearData(eventData);
                break;
            case "STAGE_FAILED":
                AppendStageFailData(eventData, playerQuit);
                break;
        }

        AnalyticsManager.SendEvent(eventName, eventData);
    }

    private Dictionary<string, object> BuildBaseAnalyticsData()
    {
        var data = new Dictionary<string, object>
        {
            { "STAGE_NUMBER", stageIndex + 1 },
        };

        for (int i = 0; i < 5; i++)
        {
            string key = $"TOWER_TYPE_USED_{i + 1}";
            string value = (i < selectedTowers.Count) ? DataManager.Instance.towerDict[selectedTowers[i]].name : "None";
            data.Add(key, value);
        }

        return data;
    }

    private void AppendStageStartData(Dictionary<string, object> data)
    {
        data.Add("PLAYER_HASGOLD", SaveManager.Instance.playerData.gold);
    }

    private void AppendStageClearData(Dictionary<string, object> data)
    {
        data.Add("HAS_ARTIFACT", abilityManager.allAbilities.Values.Count);
    }

    private void AppendStageFailData(Dictionary<string, object> data, bool playerQuit)
    {
        AppendStageClearData(data); // 공통 필드

        float remainGoals = 1f - (float)floorCount / maxFloor;
        data.Add("REMAIN_GOALS", remainGoals);
        data.Add("FLOOR_NUMBER", floorCount + 1);
        data.Add("WAVE_NUMBER", CurFloor?.GetCurWave() + 1 ?? -1);
        data.Add("DID_PLAYER_QUIT", playerQuit);
    }

    void Init()
    {
        timeScaleManager = new TimeScaleManager();
        abilityManager = new AbilityManager();
        eventManager = new EventManager();

        skillTargetingSystem = gameObject.AddComponent<SkillTargetingSystem>();
        skillVisualDB = gameObject.AddComponent<SkillVisualDB>();

        stageIndex = SaveManager.Instance.playerData.selectedStageIndex;
        maxFloor = DataManager.Instance.stageDict[stageIndex].floorCount;

        UIManager.Instance.HideUI<UIPause>();
        UIManager.Instance.HideUI<UIFloorIntro>();

        var ui = UIManager.Instance.GetUI<UIFloorIntro>();
        ui.Init(maxFloor);
    }

    public void LevelScaling()
    {
        float stageValue = 1;
        float floorValue = 1;
        float waveValue = 1;
        switch (stageIndex)
        {
            case 0:
                stageValue = 1.0f;
                break;
            case 1:
                stageValue = 1.05f;
                break;
            case 2:
                stageValue = 1.12f;
                break;
            default:
                stageValue = 1.2f;
                break;
        }
        switch (floorCount)
        {
            case 0:
                floorValue = 1.0f;
                break;
            case 1:
                floorValue = 1.05f;
                break;
            case 2:
                floorValue = 1.13f;
                break;
            case 3:
                floorValue = 1.2f;
                break;
            case 4:
                floorValue = 1.3f;
                break;
            default:
                floorValue = 1.3f;
                break;
        }
        switch (CurFloor.curWave)
        {
            case 0:
                waveValue = 1.0f;
                break;
            case 1:
                waveValue = 1.15f;
                break;
            case 2:
                waveValue = 1.3f;
                break;
            default:
                waveValue = 1.3f;
                break;
        }

        monsterLevelScaling = stageValue * floorValue * waveValue;
    }

    void ApplyArtifact()
    {
        //나중에 고쳐야 함 => List<StatType, Stat> 활용
        foreach (ArtifactLevelData artifactLevel in SaveManager.Instance.artifactLevelDict.Values)
        {
            int id = artifactLevel.id;
            ArtifactData artifact = DataManager.Instance.artifactDicts[id / 1000][id];

            StatType statType = (StatType)artifact.valueType;
            float value = artifact.value;

            switch (statType)
            {
                case StatType.startCost:
                    startCost += value;
                    break;
                case StatType.costHeal:
                    costRecoveryMultiplier *= (1 + value);
                    break;
                case StatType.playerHP:
                    maxHp = (int)(maxHp * (1 + value));
                    break;
                case StatType.playerHeal:
                    healAdd = (int)(healAdd * (1 + value));
                    break;
                case StatType.atk:
                    CurHero.AddStatFromArtifact(value);
                    break;
                case StatType.cleargoldDrop:
                    baseTokenAdd += (int)value;
                    break;
                default:
                    break;
            }
        }
    }

    public int GetStageNum()
    {
        return stageIndex;
    }

    public int GetFloorNum()
    {
        return floorCount;
    }

    public int GetMaxFloor()
    {
        return maxFloor;
    }

    public int GetMaxWaveCount()
    {
        return DataManager.Instance.floorDict[floorId].waveCount;
    }

    public int GetHP()
    {
        return hp;
    }

    public int GetMaxHP()
    {
        abilityMultiplier.TryGetValue((int)StatType.playerHP, out float ratio);

        return maxHp + (int)(maxHp * ratio);
    }

    public void TakeDamage(int damage)
    {
        hp = Mathf.Max(hp - damage, 0);
        OnPlayerHpChanged.RaiseEvent(hp);
        UIManager.Instance.GetUI<UI_Hud>().TakeDamage();
        if (hp <= 0) GameOver();
    }

    public void Heal(int amount)
    {
        hp = Mathf.Min(hp + amount, GetMaxHP());
        OnPlayerHpChanged.RaiseEvent(hp);
    }

    public void AddAbilityMultiplier(int statType, float value)
    {
        if (!abilityMultiplier.TryAdd(statType, value))
        {
            abilityMultiplier[statType] += value;
        }
    }

    public void RemoveAbilityMultiplier(int statType, float value)
    {
        if (!abilityMultiplier.TryGetValue(statType, out float statValue))
        {
            abilityMultiplier[statType] -= value;
        }
            
    }

    public void AddCostRecoveryMultiplier(float value)
    {
        activeCostRecoveryMultipliers.Add(value);
        UpdateCostRecoveryMultiplier();
    }

    public void RemoveCostRecoveryMultiplier(float value)
    {
        activeCostRecoveryMultipliers.Remove(value);
        UpdateCostRecoveryMultiplier();
    }

    private void UpdateCostRecoveryMultiplier()
    {
        // 여러 타워에서 오는 버프들을 합산해서 적용
        float totalMultiplier = 1f;
        foreach (var multiplier in activeCostRecoveryMultipliers)
        {
            totalMultiplier += multiplier;
        }

        // 그에 맞춰서 속도를 조정 (누적 값 적용)
        costRecoveryMultiplier = totalMultiplier;
    }

    IEnumerator RegainCost()
    {
        while (true)
        {
            abilityMultiplier.TryGetValue((int)StatType.costHeal, out float ratio);

            curCost = curCost + Time.deltaTime * (costRecoveryMultiplier + (costRecoveryMultiplier * ratio));
            OnCostChanged.RaiseEvent(curCost);
            yield return null;
        }
    }

    public bool CheckCost(int amount)
    {
        if (curCost < amount) return false;
        return true;
    }

    public bool UseCost(int amount)
    {
        if (CheckCost(amount))
        {
            curCost -= amount;
            return false;
        }

        return true;
    }

    public void GetCost(int amount)
    {
        curCost += amount;
    }

    public void GainToken(int amount)
    {
        token += amount;
    }

    public bool CheckToken(int amount)
    {
        return token >= amount;
    }

    public void UseToken(int amount)
    {
        if (CheckToken(amount)) token -= amount;
    }

    public void GainBook(int amount)
    {
        book += amount;
    }

    public bool CheckBook(int amount)
    {
        return book >= amount;
    }

    public void UseBook(int amount)
    {
        if (CheckBook(amount)) book -= amount;
    }

    void GameOver()
    {
        Debug.Log("게임오버!");

        StopCoroutine(stageCoroutine);

        timeScaleManager.PushTimeScale(0f);
        var ui = UIManager.Instance.ShowUI<UI_Wave>();
        ui.ShowFaildText(() =>
        {
            timeScaleManager.PopTimeScale();
            EndStage();
        });
    }

    public void ResetDropTowerCooldown()
    {
        OnResetTowerCoolDown.RaiseEvent();
    }

    public void StartStage()
    {
        StartCoroutine(RegainCost());
        stageCoroutine = StartCoroutine(ProgressStage());
    }

    IEnumerator ProgressStage()
    {
        Debug.Log("<color=white>스테이지 시작</color>");
        List<int> floorDictKeys = new(DataManager.Instance.stageDict[stageIndex].floorPool);
        isFadeComplete = true;

        for (int i = 0; i < maxFloor; i++)
        {
            UIManager.Instance.HideUI<UITowerInfo>();

            if (i != 0)
            {
                //book = 100;
                //token = 100000;
                ShowFloorLoading();
            }

            OnFloorCountChanged.RaiseEvent(i + 1);

            // 화면이 전부 어두워진 후 다음 맵 로드
            yield return new WaitUntil(() => isFadeComplete);

            if (floorGO != null) Destroy(floorGO);
            if (i == maxFloor - 1)
            {
                floorId = DataManager.Instance.stageDict[stageIndex].bossFloorID;
            }
            else
            {
                int randomIndex = Random.Range(0, floorDictKeys.Count);
                floorId = floorDictKeys[randomIndex];
                floorDictKeys.RemoveAt(randomIndex);
            }

            int mapID = DataManager.Instance.floorDict[floorId].mapID;
            floorGO = Util.InstantiatePrefab($"Floors/Floor_{mapID}"); //랜덤 ID에 맞는 플로어 생성하게 변경해야 함
            curFloor = floorGO.GetComponent<Floor>();

            yield return new WaitUntil(() => isIntroEnd);
           
            curFloor.StartFloor(floorId);
            curCost = startCost;

            yield return new WaitUntil(() => curFloor.isFloorEnd);
            //플로어 클리어 시점
            curCost = 0;
            floorCount += 1;
            GainToken(baseTokenAdd + floorCount * floorTokenAdd);

            if (i != 0 && (i + 1) % 2 == 0)
            {
                //ShowFloorIntro();
                //yield return new WaitUntil(() => isIntroEnd);

                ShowEvent();
                yield return new WaitUntil(() => isEventEnd);

                // 추가 플로어 이벤트 발생 시
                //if (isAdditionalFloor)
                //{
                //    StartCoroutine(AdditionalStageRoutine(floorDictKeys));

                //    yield return new WaitUntil(() => curFloor.isFloorEnd);
                //}
            }
        }

        EndStage();
    }

    //IEnumerator AdditionalStageRoutine(List<int> floorDictKeys)
    //{
    //    if (floorGO != null) Destroy(floorGO);
    //    int randomIndex = Random.Range(0, floorDictKeys.Count);
    //    int floorId = floorDictKeys[randomIndex];
    //    floorDictKeys.RemoveAt(randomIndex);
    //    int mapID = DataManager.Instance.floorDict[floorId].mapID;
    //    floorGO = Util.InstantiatePrefab($"Floors/Floor_{mapID}");//랜덤 ID에 맞는 플로어 생성하게 변경해야 함
    //    curFloor = floorGO.GetComponent<Floor>();
    //    curFloor.StartFloor(floorId);

    //    //yield return new WaitUntil(() => curFloor.isFloorEnd);
    //    yield return null;

    //    isAdditionalFloor = false;
    //}

    void ShowEvent()
    {
        Debug.Log("<color=white>이벤트 선택</color>");
        int stepNum = Util.GetFunnelStepForEventSelect(floorCount);

        AnalyticsManager.SendEvent("Funnel_Step", new Dictionary<string, object>
        {
            { "Funnel_Step_Number", stepNum }
        });
        
        isEventEnd = false;
        eventManager.ShowEvent();
    }

    void ShowFloorLoading()
    {
        Debug.Log("<color=white>플로어 진입 인트로</color>");

        isIntroEnd = false;
        isFadeComplete = false;

        // 플로어 종료 시 FadeOut
        var ui = UIManager.Instance.ShowUI<UI_Fade>();
        ui.FadeOut(() =>
        {
            ui.FadeIn();
            
            int stepNum = Util.GetFunnelStepForShopEntry(floorCount);
            Debug.Log($"<color=white><UNK> <UNK> {stepNum} <UNK></color>");
            AnalyticsManager.SendEvent("Funnel_Step", new Dictionary<string, object>
            {
                { "Funnel_Step_Number", stepNum }
            });
            
            isFadeComplete = true;
            UIManager.Instance.ShowUI<UI_FloorLoading>();
        });
    }

    int GetReward()
    {
        int rewardGold =
            (int)(DataManager.Instance.stageDict[stageIndex].rewardGold * floorCount / (float)maxFloor);
        if (floorCount >= maxFloor) rewardGold *= 2;
        SaveManager.Instance.playerData.AddGold(rewardGold);
        Debug.Log($"<color=white>{rewardGold}골드 지급</color>");

        return rewardGold;
    }

    void EndStage()
    {
        //보상 챙겨주고 로비로 보내야 함
        Debug.Log("<color=white>스테이지 종료</color>");

        bool isSuccess = floorCount >= maxFloor;
        
        string eventName = isSuccess ? "STAGE_CLEARED" : "STAGE_FAILED";

        SendAnalytics(eventName);
        
        UIManager.Instance.ShowUI<UI_StageResult>().Init(isSuccess, (int)timer, floorCount, GetReward());
        if (isSuccess)
            SaveManager.Instance.playerData.AddStage(SaveManager.Instance.playerData.selectedStageIndex + 1);

        timeScaleManager.PushTimeScale(0f);
    }
}