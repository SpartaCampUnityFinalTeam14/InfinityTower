using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : Singleton<StageManager>
{
    private int hp;
    private float curCost = 100f;
    //private float maxCost = 10f;
    [SerializeField] private float costRecoveryMultiplier = 100f;  // Cost 얻는 속도 - 기본 1배속
    [SerializeField] private FloatEventChannel OnCostChanged;
    private List<float> activeCostRecoveryMultipliers = new List<float>(); // 여러 타워의 버프들을 저장

    public List<int> selectedTowers = new();
    public int selectedChampion;

    public AbilityManager abilityManager;
    public EventManager eventManager;

    [SerializeField] private int floorCount = 2;
    private GameObject floorGO;
    private Floor curFloor;
    public Floor CurFloor => curFloor;
    [SerializeField] private IntEventChannel OnFloorCountChanged;

    public bool isEventEnd;
    public bool isPause;

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
        UIManager.Instance.HideUI<UIPause>();

        abilityManager = gameObject.AddComponent<AbilityManager>();
        eventManager = gameObject.AddComponent<EventManager>();
    }

    public void AddFloorCount(int count)
    {
        floorCount += count;
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

    

    public void StartStage()
    {
        StartCoroutine(RegainCost());
        StartCoroutine(ProgressStage());
    }

    IEnumerator ProgressStage()
    {
        Debug.Log("<color=white>스테이지 시작</color>");

        for(int i = 0; i < floorCount; i++)
        {
            OnFloorCountChanged.RaiseEvent(i + 1);

            if(floorGO != null) Destroy(floorGO);
            floorGO = Util.InstantiatePrefab("Floors/TestFloor");//랜덤 ID에 맞는 플로어 생성하게 변경해야 함
            curFloor = floorGO.GetComponent<Floor>();
            curFloor.StartFloor();

            curCost = 0;

            yield return new WaitUntil(() => curFloor.isFloorEnd);

            //if (i != 0 && (i + 1) % 2 == 0) ShowEvent();
            ShowEvent();

            yield return new WaitUntil(() => isEventEnd);
        }

        EndStage();
    }
   
    void ShowEvent()
    {
        Debug.Log("<color=white>이벤트 선택</color>");

        eventManager.ShowEvent();
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
