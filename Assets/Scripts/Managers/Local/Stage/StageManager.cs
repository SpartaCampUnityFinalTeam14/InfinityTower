using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public float curCost;
    private float maxCost;
    //public List<Tower> towers 선택한 타워 리스트
    //public Champion champion 선택한 영웅
    //public List<Perk> perks 선택한 특성 리스트

    [SerializeField] private int floorCount = 2;
    private GameObject floorGO;
    private Floor curFloor;

    private void Awake()
    {
        //영웅 세팅 필요

        StartStage();//추후 awake가 아닌 다른 곳으로 이동 (예를 들어, 시작 버튼을 누른다든가 하는 식)
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
