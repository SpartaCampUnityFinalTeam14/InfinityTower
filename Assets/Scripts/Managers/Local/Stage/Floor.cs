using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Floor : MonoBehaviour
{
    //플로어
    private FloorData floorData;

    [SerializeField] private Tilemap pathTilemap;
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;

    //public Vector2 spawnPosition;
    public float waveStartDelayTime;
    public bool isFloorEnd;
    public bool isPerkSelected;

    //웨이브
    private WaveData waveData;

    public bool isWaveEnd;

    public void Init(FloorData data)
    {
        floorData = data;

        //spawnPosition = new Vector2(DataManager.Instance.floorDict[0].spawnPosition[0]
        //    , DataManager.Instance.floorDict[0].spawnPosition[1]);
        waveStartDelayTime = 1;

        PathManager.Instance.Init(pathTilemap, startPos, endPos);
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
            yield return new WaitForSeconds(waveStartDelayTime);

            StartWave(floorData.waveID[i]);

            yield return new WaitUntil(() => isWaveEnd);

            SelectPerk();
        }

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
        spawnedMonster.Init(monsterID, PathManager.Instance.pathPoints, startPos);

        //테스트 코드, 나중에는 몬스터의 Init()에 스프라이트와 애니메이션 변경해주는 코드 추가해야 함
        float color = (float)monsterID / DataManager.Instance.monsterDict.Count;
        spawnedMonster.GetComponentInChildren<SpriteRenderer>().color = new Color(color, color, color);

        Debug.Log("<color=red>몬스터 스폰함</color>");
    }

    void SelectPerk()
    {
        Debug.Log("<color=green>특성 선택</color>");

        //구현해야 함
    }

    void EndWave()
    {
        Debug.Log("<color=green>웨이브 종료</color>");

        isWaveEnd = true;
    }
}
