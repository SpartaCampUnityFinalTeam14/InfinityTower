using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class Floor : MonoBehaviour
{
    private PathManager path;

    //플로어
    [SerializeField] private int id;
    private FloorData floorData;

    //public Vector2 spawnPosition;
    [SerializeField] private float waveStartDelayTime;
    public bool isFloorEnd;
    public bool isPerkSelected;

    //웨이브
    private WaveData waveData;

    private int monsterCnt = 0;
    private bool isWaveEnd;

    [SerializeField] private IntEventChannel OnWaveCountChanged;
    [SerializeField] private IntEventChannel OnMonsterCountChanged;

    private void Awake()
    {
        path = GetComponent<PathManager>();

        waveStartDelayTime = 1;
        floorData = DataManager.Instance.floorDict[id];
    }

    public void StartFloor()
    {
        Debug.Log("<color=cyan>플로어 시작</color>");

        isFloorEnd = false;
        StartCoroutine(ProgressFloor());
    }

    IEnumerator ProgressFloor()
    {
        for (int i = 0; i < floorData.waveCount; i++)
        {
            OnWaveCountChanged.RaiseEvent(i + 1);

            yield return new WaitForSeconds(waveStartDelayTime);

            StartWave(floorData.waveID[i]);

            yield return new WaitUntil(() => isWaveEnd);

            SelectPerk();
            yield return new WaitUntil(() => isPerkSelected);
        }

        yield return new WaitUntil(() => monsterCnt <= 0);

        EndFloor();
    }

    void EndFloor()
    {
        Debug.Log("<color=cyan>플로어 종료</color>");

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

        EndWave();
    }

    void SpawnMonster(int monsterID)
    {
        GameObject monster = Resources.Load<GameObject>($"Prefabs/Monsters/TestMonster");
        MonsterBase spawnedMonster = PoolManager.Instance.Get(monster).GetComponent<MonsterBase>();
        spawnedMonster.Init(monsterID, path.pathPoints, path.startPos, this);
        AddMonsterCount(1);

        //테스트 코드, 나중에는 몬스터의 Init()에 스프라이트와 애니메이션 변경해주는 코드 추가해야 함
        float color = (float)monsterID / DataManager.Instance.monsterDict.Count;
        spawnedMonster.GetComponentInChildren<SpriteRenderer>().color = new Color(color, color, color);

        Debug.Log("<color=red>몬스터 스폰함</color>");
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
        uiAbility.Show();
        uiAbility.DrawAbility();
    }

    void EndWave()
    {
        Debug.Log("<color=green>웨이브 종료</color>");

        isWaveEnd = true;
    }
}
