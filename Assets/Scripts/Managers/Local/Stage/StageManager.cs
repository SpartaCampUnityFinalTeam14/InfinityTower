using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : Singleton<StageManager>
{
    private int hp;
    private float curCost = 1f;
    public float CurrentCost => curCost;

    //private float maxCost = 10f;
    [SerializeField] private float costRecoveryMultiplier = 100f;  // Cost 얻는 속도 - 기본 1배속
    [SerializeField] private FloatEventChannel OnCostChanged;
    private List<float> activeCostRecoveryMultipliers = new List<float>(); // 여러 타워의 버프들을 저장

    [HideInInspector] public List<int> selectedTowers = new();
    [HideInInspector] public int selectedChampion;

    [HideInInspector] public AbilityManager abilityManager;
    [HideInInspector] public EventManager eventManager;
    [HideInInspector] public TimeScaleManager timeScaleManager;

    [HideInInspector] public SkillTargetingSystem skillTargetingSystem;
    [HideInInspector] public SkillVisualDB skillVisualDB;
    private Hero hero;
    
    [SerializeField] private HeroSkillPanel skillPanel;

    [SerializeField] private int floorCount = 2;
    private GameObject floorGO;
    private Floor curFloor;
    public Floor CurFloor => curFloor;
    [SerializeField] private IntEventChannel OnFloorCountChanged;

    private List<TowerSlotUI> towerSlots;

    [HideInInspector] public bool isEventEnd;
    [HideInInspector] public bool isIntroEnd;
    [HideInInspector] public bool isAdditionalFloor;

    protected override void Awake()
    {
        isGlobal = false;

        base.Awake();

        //영웅 스킬 세팅 필요
        selectedTowers = SaveManager.Instance.playerData.selectedTowerIndex;
        selectedChampion = SaveManager.Instance.playerData.selectedChampionIndex;
        hp = DataManager.Instance.championDict[selectedChampion].hp;
        
        //abilityManager = GetComponent<AbilityManager>();
        Init();
        StartStage();//추후 awake가 아닌 다른 곳으로 이동 (예를 들어, 시작 버튼을 누른다든가 하는 식)
    }

    void Init()
    {
        timeScaleManager = new TimeScaleManager();
        abilityManager = new AbilityManager();
        eventManager = new EventManager();

        towerSlots = new List<TowerSlotUI>();

        skillTargetingSystem = gameObject.AddComponent<SkillTargetingSystem>();
        skillVisualDB = gameObject.AddComponent<SkillVisualDB>();

        //InitHero();
        // tower skill panel 생성
        UIManager.Instance.ShowUI<StageEntryUI>();

        UIManager.Instance.HideUI<UIPause>();
        UIManager.Instance.HideUI<UIFloorIntro>();

        var ui = UIManager.Instance.GetUI<UIFloorIntro>();
        ui.Init(floorCount);
    }
    
    public void InitHero(HeroSkillPanel skillPanel)
    {
        this.skillPanel = skillPanel;

        hero = new Hero();
        Debug.Log("👤 챔피언 생성됨: " + selectedChampion);

        ChampionData champData = DataManager.Instance.championDict[selectedChampion];

        Debug.Log($"{champData.skillId.Count} 개의 스킬을 가지고 있습니다.");

        foreach (int skillId in champData.skillId)
        {
            if (DataManager.Instance.skillDict.TryGetValue(skillId, out SkillData skillData))
            {
                Debug.Log($"⚡ 스킬 ID: {skillId}");
                Skill skill = SkillFactory.CreateSkill(skillData);
                if (skill != null)
                    hero.skills.Add(skill);
            }
            else
            {
                Debug.LogWarning($"⚠️ SkillData ID {skillId} 를 찾을 수 없습니다.");
            }
        }

        // ✅ 영웅 스킬 패널에 연결 (SerializeField 연결 기준)
        if (skillPanel != null)
        {
            skillPanel.InitHero(hero);
        }
        else
        {
            Debug.LogWarning("⚠️ HeroSkillPanel이 연결되어 있지 않습니다!");
        }
    }

    public void TakeDamage(int damage)
    {
        hp = Mathf.Max(hp - damage, 0);
        Debug.Log(hp);
        if (hp <= 0) GameOver();
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
            curCost = curCost + Time.deltaTime * costRecoveryMultiplier;
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

    void GameOver()
    {
        Debug.Log("게임오버!");

        EndStage();
    }

    public void AddTowerSlot(TowerSlotUI towerSlot)
    {
        towerSlots.Add(towerSlot);
    }

    public void ResetDropTowerCooldown()
    {
        foreach (var towerSlot in towerSlots)
        {
            towerSlot.ResetCooldown();
        }
    }

    public void StartStage()
    {
        StartCoroutine(RegainCost());
        StartCoroutine(ProgressStage());
    }

    IEnumerator ProgressStage()
    {
        Debug.Log("<color=white>스테이지 시작</color>");
        List<int> floorDictKeys = DataManager.Instance.floorDict.Keys.ToList();

        for (int i = 0; i < floorCount; i++)
        {
            if (i != 0)
            {
                ShowFloorIntro();
            }

            OnFloorCountChanged.RaiseEvent(i + 1);

            if (floorGO != null) Destroy(floorGO);
            int randomIndex = Random.Range(0, floorDictKeys.Count);
            int floorId = floorDictKeys[randomIndex];
            floorDictKeys.RemoveAt(randomIndex);
            floorGO = Util.InstantiatePrefab($"Floors/Floor_{floorId}");//랜덤 ID에 맞는 플로어 생성하게 변경해야 함
            curFloor = floorGO.GetComponent<Floor>();

            yield return new WaitUntil(() => isIntroEnd);
            
            curFloor.StartFloor();
            curCost = 0;

            yield return new WaitUntil(() => curFloor.isFloorEnd);
            curCost = 0;

            if (i != 0 && (i + 1) % 2 == 0)
            {
                ShowFloorIntro();
                yield return new WaitUntil(() => isIntroEnd);

                ShowEvent();
                yield return new WaitUntil(() => isEventEnd);
            }

            // 추가 플로어
            if (isAdditionalFloor)
            {
                StartAdditionalFloor();

                yield return new WaitUntil(() => !isAdditionalFloor);
            }
        }

        EndStage();
    }

    void StartAdditionalFloor()
    {
        StartCoroutine(AdditionalStageRoutine());
    }

    IEnumerator AdditionalStageRoutine()
    {
        if (floorGO != null) Destroy(floorGO);
        floorGO = Util.InstantiatePrefab("Floors/TestFloor");
        curFloor = floorGO.GetComponent<Floor>();
        curFloor.StartFloor();

        yield return new WaitUntil(() => curFloor.isFloorEnd);

        isAdditionalFloor = false;
    }

    void ShowEvent()
    {
        Debug.Log("<color=white>이벤트 선택</color>");

        isEventEnd = false;
        eventManager.ShowEvent();
    }

    void ShowFloorIntro()
    {
        Debug.Log("<color=white>플로어 진입 인트로</color>");

        isIntroEnd = false;
        var ui = UIManager.Instance.ShowUI<UIFloorIntro>();
        //ui.Show();
    }

    void GetReward()
    {
        Debug.Log("<color=white>골드 지급</color>");
        //골드 챙겨줘야 함
    }

    void EndStage()
    {
        //보상 챙겨주고 로비로 보내야 함
        GetReward();
        Debug.Log("<color=white>스테이지 종료</color>");

        GameManager.Instance.LoadScene("KSM_Lobby");
    }
}
