using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : Singleton<StageManager>
{
    public float curCost;
    private float maxCost;
    public List<int> selectedTowers = new();
    public int selectedChampion;
    public Dictionary<int, Dictionary<int, AbilityData>> filterAbilityPool; // 특성 가챠에 사용될 특성 풀 (Dictionary<레어도, Dictionary<특성ID, 특성데이터>>)
    public Dictionary<int, AbilityData> ability; // 선택한 특성 리스트 <특성id, 특성>

    [SerializeField] private int floorCount = 2;
    private GameObject floorGO;
    private Floor curFloor;
    public Floor CurFloor => curFloor;

    protected override void Awake()
    {
        isGlobal = false;
        base.Awake();

        //영웅 세팅 필요
        selectedTowers = SaveManager.Instance.playerData.selectedTowerIndex;
        selectedChampion = SaveManager.Instance.playerData.selectedChampionIndex;

        FilterAbilitiesByDeck(); // 현재 덱에 따라 특성 필터링
        StartStage();//추후 awake가 아닌 다른 곳으로 이동 (예를 들어, 시작 버튼을 누른다든가 하는 식)
    }

    public void FilterAbilitiesByDeck()
    {
        // Ditionary 초기화 작업
        ability = new Dictionary<int, AbilityData>();
        filterAbilityPool = new Dictionary<int, Dictionary<int, AbilityData>>();
        var abilityDatas = DataManager.Instance.abilityDict;
        foreach (var data in abilityDatas.Values)
        {
            if (!filterAbilityPool.ContainsKey(data.rarity))
                filterAbilityPool.Add(data.rarity, new Dictionary<int, AbilityData>());

            filterAbilityPool[data.rarity].Add(data.id ,data.DeepCopy());
        }

        // 현재 덱에 관련된 특성만 남기기
        List<int> removeKey = new List<int>();
        foreach (var ability in filterAbilityPool.Values)
        {
            removeKey.Clear();

            foreach (var data in ability.Values)
            {
                if (data.targetID != -1 && data.targetType.Equals((int)TargetType.Tower) && !selectedTowers.Contains(data.targetID))
                    removeKey.Add(data.id);
            }

            foreach (var key in removeKey)
            {
                ability.Remove(key);
            }
        }
    }

    public void StartStage()
    {
        StartCoroutine(ProgressStage());
    }

    IEnumerator ProgressStage()
    {
        Debug.Log("<color=white>스테이지 시작</color>");

        for(int i = 0; i < floorCount; i++)
        {
            if(floorGO != null) Destroy(floorGO);
            floorGO = Util.InstantiatePrefab("testFloor");
            floorGO.GetComponentInChildren<SpriteRenderer>().color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            curFloor = floorGO.AddComponent<Floor>();
            curFloor.Init(DataManager.Instance.floorDict[Random.Range(0, DataManager.Instance.floorDict.Count)]);
            curFloor.StartFloor();

            yield return new WaitUntil(() => curFloor.isFloorEnd);

            if (i % 2 == 0) ShowEvent();
        }

        EndStage();
    }
   
    void ShowEvent()
    {
        Debug.Log("<color=white>이벤트 선택</color>");
        //구현해야 함
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

        SceneManager.LoadScene("KSM_Lobby");
    }
}
