using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : Singleton<StageManager>
{
    public int hp;
    public float curCost;
    private float maxCost;

    public List<int> selectedTowers = new();
    public int selectedChampion;
    //public List<Perk> perks 선택한 특성 리스트

    [SerializeField] private int floorCount = 2;
    private GameObject floorGO;
    private Floor curFloor;

    protected override void Awake()
    {
        isGlobal = false;

        base.Awake();

        //영웅 스킬 세팅 필요
        selectedTowers = SaveManager.Instance.playerData.selectedTowerIndex;
        selectedChampion = SaveManager.Instance.playerData.selectedChampionIndex;
        hp = DataManager.Instance.championDict[selectedChampion].hp;

        StartStage();//추후 awake가 아닌 다른 곳으로 이동 (예를 들어, 시작 버튼을 누른다든가 하는 식)
    }

    public void TakeDamage(int damage)
    {
        hp = Mathf.Max(hp - damage, 0);

        if (hp <= 0) GameOver();
    }

    void GameOver()
    {
        Debug.Log("게임오버!");

        EndStage();
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
            floorGO = Util.InstantiatePrefab("Floors/TestFloor");//랜덤 ID에 맞는 플로어 생성하게 변경해야 함
            curFloor = floorGO.GetComponent<Floor>();
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
