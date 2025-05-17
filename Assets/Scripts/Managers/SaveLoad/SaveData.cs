//캐릭터 정보 세이브 로드 용도
using System;
using System.Collections.Generic;

[Serializable]
public abstract class ISaveLoader<Key, Value>
{
    public List<Value> data = new List<Value>();
    public abstract Dictionary<Key, Value> MakeDict();
}

#region PlayerData
[Serializable]
public class PlayerData
{
    public int gold;
    public int selectedStageIndex;
    public List<int> selectedTowerIndex;
    public int selectedChampionIndex;

    public PlayerData()
    {
        gold = 1000;
        selectedStageIndex = 0;
        selectedTowerIndex = new List<int>(5) { 0, -1, -1, -1, -1 };
        selectedChampionIndex = 0;
    }

    public bool CheckGold(int amount)
    {
        if (amount <= gold) return true;
        else return false;
    }

    public bool UseGold(int amount)
    {
        if (CheckGold(amount))
        {
            gold -= amount;
            SaveManager.Instance.SavePlayerData();
            
            return true;
        }
        else return false;
    }

    public void AddGold(int amount)
    {
        gold += amount;
        SaveManager.Instance.SavePlayerData();
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

#region TowerLevelData
[Serializable]
public class TowerLevelData
{
    public int id;
    public int level;
    public int exp;

    public TowerLevelData(int id, int level, int exp)
    {
        this.id = id;
        this.level = level;
        this.exp = exp;
    }
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

#region ChampionLevelData
[Serializable]
public class ChampionLevelData
{
    public int id;
    public int level;
    public int exp;

    public ChampionLevelData(int id, int level, int exp)
    {
        this.id = id;
        this.level = level;
        this.exp = exp;
    }
}

[Serializable]
public class ChampionLevelDataLoader : ISaveLoader<int, ChampionLevelData>
{
    public override Dictionary<int, ChampionLevelData> MakeDict()
    {
        Dictionary<int, ChampionLevelData> dict = new();
        foreach (ChampionLevelData championLevel in data)
        {
            dict.Add(championLevel.id, championLevel);
        }

        return dict;
    }
}
#endregion

#region ArtifactSaveData
[Serializable]
public class ArtifactLevelData
{
    public int id;
    public int count;

    public ArtifactLevelData(int id, int count)
    {
        this.id = id;
        this.count = count;
    }
}

[Serializable]
public class ArtifactLevelDataLoader : ISaveLoader<int, ArtifactLevelData>
{
    public override Dictionary<int, ArtifactLevelData> MakeDict()
    {
        Dictionary<int, ArtifactLevelData> dict = new();
        foreach (ArtifactLevelData artifact in data)
        {
            dict.Add(artifact.id, artifact);
        }

        return dict;
    }
}
#endregion