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
        selectedTowerIndex = new List<int>(5) {0,0,0,0,0};
        selectedChampionIndex = 0;
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