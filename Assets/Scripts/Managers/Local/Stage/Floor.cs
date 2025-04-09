using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    //플로어
    private FloorData floorData;

    public Vector2 spawnPosition;
    public float waveStartDelayTime;
    public bool isFloorEnd;
    public bool isPerkSelected;

    //웨이브
    private WaveData waveData;

    public bool isWaveEnd;
    public float minSpawnDelayTime;

    public void Init(FloorData data)
    {
        floorData = data;

        spawnPosition = new Vector2(DataManager.Instance.floorDict[0].spawnPosition[0]
            , DataManager.Instance.floorDict[0].spawnPosition[1]);
        waveStartDelayTime = 1;
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
            yield return new WaitUntil(() => isPerkSelected);
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

    bool SpawnMonster(int monsterID)
    {
        MonsterData monsterData = DataManager.Instance.monsterDict[monsterID];

        Debug.Log($"<color=red>{monsterData.name} 생성됨</color>");
        //스폰 위치에 몬스터 생성해야 함

        return true;
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
