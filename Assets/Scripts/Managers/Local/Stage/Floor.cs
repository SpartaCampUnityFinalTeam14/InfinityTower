using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class Floor : MonoBehaviour
{
    private PathManager path;

    //플로어
    private int id;
    private FloorData floorData;

    //public Vector2 spawnPosition;
    [SerializeField] private float waveStartDelayTime;
    public bool isFloorEnd;
    public bool isPerkSelected;

    //웨이브
    private WaveData waveData;
    public int curWave;

    [SerializeField] int monsterCnt = 0;
    private bool isWaveEnd;

    // 플로어에 소환된 타워 리스트
    private List<BaseTower> spawnTowerList;

    [SerializeField] private IntEventChannel OnWaveCountChanged;
    [SerializeField] private IntEventChannel OnMonsterCountChanged;

    private void Awake()
    {
        path = GetComponent<PathManager>();

        waveStartDelayTime = 1;
    }

    private void Start()
    {
        spawnTowerList = new List<BaseTower>();

        // 특성 초기화
        UIManager.Instance.HideUI<UIAbility>();

        OnWaveCountChanged.RaiseEvent(1);
    }

    public void StartFloor(int floorId)
    {
        Debug.Log("<color=cyan>플로어 시작</color>");

        floorData = DataManager.Instance.floorDict[floorId];
        isFloorEnd = false;
        
        // ✅ 퍼널 이벤트 전송
        int floorNum = StageManager.Instance.GetFloorNum(); // 0부터 시작 (플로어1 = 0)

        int stepNum = Util.GetFunnelStepForFloorStart(floorNum);
        
        Debug.Log($"<color=cyan>플로어 시작</color> {floorNum}, {stepNum}");
        
        AnalyticsManager.SendEvent("Funnel_Step", new Dictionary<string, object>
        {
            { "Funnel_Step_Number", stepNum }
        });
        
        StartCoroutine(ProgressFloor());
    }

    IEnumerator ProgressFloor()
    {
        for (curWave = 0; curWave < floorData.waveCount; curWave++)
        {
            StageManager.Instance.LevelScaling();

            var ui = UIManager.Instance.ShowUI<UI_Wave>();
            ui.ShowWaveNum(curWave + 1);

            OnWaveCountChanged.RaiseEvent(curWave + 1);

            yield return new WaitForSeconds(waveStartDelayTime);
            
            // ✅ 웨이브 시작 퍼널 이벤트 전송
            int floorIndex = StageManager.Instance.GetFloorNum(); // 0~
            int stepNum = Util.GetFunnelStepForWave(floorIndex, curWave);
            
            Debug.Log($"<color=cyan><UNK> 웨이브 시작 {floorIndex}, {stepNum} <UNK></color>");
            
            AnalyticsManager.SendEvent("Funnel_Step", new Dictionary<string, object>
            {
                { "Funnel_Step_Number", stepNum }
            });

            StartWave(floorData.waveID[curWave]);

            yield return new WaitUntil(() => isWaveEnd);
            //웨이브 종료 시점
            StageManager.Instance.GainBook(1);

            //SelectPerk();
            //yield return new WaitUntil(() => isPerkSelected);
        }

        EndFloor();
    }

    void EndFloor()
    {
        Debug.Log("<color=cyan>플로어 종료</color>");
        
        // ✅ 퍼널 이벤트: 플로어 클리어
        int floorIndex = StageManager.Instance.GetFloorNum();
        int funnelStep = Util.GetFunnelStepForFloorClear(floorIndex);
        
        Debug.Log($"<color=cyan><UNK> 플로우 종료 {floorIndex}, {funnelStep} <UNK></color>");
        
        AnalyticsManager.SendEvent("Funnel_Step", new Dictionary<string, object>
        {
            { "Funnel_Step_Number", funnelStep }
        });
        
        ReleaseTower();
        StageManager.Instance.ResetDropTowerCooldown();

        var ui = UIManager.Instance.ShowUI<UI_Wave>();
        ui.ShowFloorClear(() => isFloorEnd = true);
    }

    public void StartWave(int index)
    {
        Debug.Log("<color=green>웨이브 시작</color>");

        isWaveEnd = false;
        waveData = DataManager.Instance.waveDict[index];
        StartCoroutine(ProgressWave());
    }

    IEnumerator ProgressWave()
    {
        for (int i = 0; i < waveData.enemyID.Count; i++)
        {
            for (int j = 0; j < waveData.spawnCount[i]; j++)
            {
                SpawnMonster(waveData.enemyID[i]);

                yield return new WaitForSeconds(waveData.spawnDelayTime[i]);
            }
        }

        yield return new WaitUntil(() => monsterCnt <= 0);
        EndWave();
    }

    void SpawnMonster(int monsterID)
    {
        GameObject monster = Resources.Load<GameObject>($"Prefabs/Monsters/Enemy_{monsterID}");
        MonsterBase spawnedMonster = PoolManager.Instance.Get(monster).GetComponent<MonsterBase>();
        spawnedMonster.Init(monsterID, path.pathPoints, path.startPos, this);
        AddMonsterCount(1);
        
        Debug.Log("<color=red>몬스터 스폰함</color>");
    }

    void ReleaseTower()
    {
        foreach (var tower in spawnTowerList) 
        {
            PoolManager.Instance.Release(tower);
        }

        spawnTowerList.Clear();
    }

    public void AddTowerInfo(BaseTower tower)
    {
        spawnTowerList.Add(tower);
    }

    public void AddMonsterCount(int count)
    {
        monsterCnt += count;
        OnMonsterCountChanged.RaiseEvent(monsterCnt);
    }

    public void SubrtactMonsterCount(int count)
    {
        monsterCnt -= count;
        OnMonsterCountChanged.RaiseEvent(monsterCnt);
    }

    void SelectPerk()
    {
        Debug.Log("<color=green>특성 선택</color>");
        isPerkSelected = false;
        var uiAbility = UIManager.Instance.ShowUI<UIAbility>();
        uiAbility.DrawAbility();
    }

    void EndWave()
    {
        Debug.Log("<color=green>웨이브 종료</color>");
        isWaveEnd = true;
    }
}
