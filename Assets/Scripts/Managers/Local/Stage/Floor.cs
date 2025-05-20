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
        StartCoroutine(ProgressFloor());
    }

    IEnumerator ProgressFloor()
    {
        for (int i = 0; i < floorData.waveCount; i++)
        {
            var ui = UIManager.Instance.ShowUI<UI_Wave>();
            ui.ShowWaveNum(i + 1);

            OnWaveCountChanged.RaiseEvent(i + 1);

            yield return new WaitForSeconds(waveStartDelayTime);

            StartWave(floorData.waveID[i]);

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
        
        ReleaseTower();
        StageManager.Instance.ResetDropTowerCooldown();

        isFloorEnd = true;
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

        var ui = UIManager.Instance.ShowUI<UI_Wave>();
        ui.ShowWaveClear(() => isWaveEnd = true);
    }
}
