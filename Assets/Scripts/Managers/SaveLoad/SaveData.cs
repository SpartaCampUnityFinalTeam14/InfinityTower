//캐릭터 정보 세이브 로드 용도
using System;
using System.Collections.Generic;

[Serializable]
public abstract class ISaveLoader<Key, Value>
{
    public abstract Dictionary<Key, Value> MakeDict();
    public List<Value> data = new List<Value>();
}

#region PlayerData
[Serializable]
public class PlayerData
{
    public int gold;
    public List<int> selectedTowerIndex;
    public int selectedChampionIndex;

    public PlayerData()
    {
        gold = 1000;
        selectedTowerIndex = new List<int>(5) { -1, -1, -1, -1, -1 };
        selectedChampionIndex = 0;
    }

    public int AddTower(int pos, int towerIndex)
    {
        int beforeId = selectedTowerIndex.IndexOf(towerIndex);

        if(beforeId != -1)
        {//선택한 타워가 다른 슬롯에 존재하고
            if (selectedTowerIndex[pos] != -1)
            {//선택한 슬롯에 이미 다른 타워가 있으면
                //선택한 슬롯에 있는 타워를 원래 슬롯으로 
                selectedTowerIndex[beforeId] = selectedTowerIndex[pos];
            }
            else
            {//선택한 슬롯이 비어 있으면
                //원래 슬롯 비우기
                selectedTowerIndex[beforeId] = -1;
            }
        }

        //선택한 타워 선택한 슬롯에 대입
        selectedTowerIndex[pos] = towerIndex;

        SaveManager.Instance.SavePlayerData();

        return beforeId;
    }
}
#endregion

#region LevelData
[Serializable]
public class TowerLevelData
{
    public int id;
    public int level;
}

[Serializable]
public class TowerLevelDataLoader : ISaveLoader<int, TowerLevelData>
{
    public override Dictionary<int, TowerLevelData> MakeDict()
    {
        Dictionary<int, TowerLevelData> dict = new();
        foreach (TowerLevelData towerLevel in data)
        {
            dict.Add(towerLevel.id, towerLevel);
        }

        return dict;
    }
}
#endregion