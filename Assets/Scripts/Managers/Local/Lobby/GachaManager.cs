using System.Collections.Generic;
using UnityEngine;

public class GachaManager
{
    private float championProb = 0.1f;

    private List<int> championPool = new();
    private List<int> towerPool = new();

    public GachaManager()
    {
        CreatePool();
    }

    void CreatePool()
    {
        foreach(var data in SaveManager.Instance.championLevelDict.Values)
        {
            if (CheckChampionExp(data.id)) championPool.Add(data.id);
        }

        foreach (var data in SaveManager.Instance.towerLevelDict.Values)
        {
            if (CheckTowerExp(data.id)) towerPool.Add(data.id);
        }
    }

    bool CheckChampionExp(int id)
    {
        int level = SaveManager.Instance.championLevelDict[id].level;
        int exp = SaveManager.Instance.championLevelDict[id].exp;
        int remainedExp = DataManager.Instance.levelUpDict[level].remainedExp;

        return exp < remainedExp;
    }

    bool CheckTowerExp(int id)
    {
        int level = SaveManager.Instance.towerLevelDict[id].level;
        int exp = SaveManager.Instance.towerLevelDict[id].exp;
        int remainedExp = DataManager.Instance.levelUpDict[level].remainedExp;

        return exp < remainedExp;
    }

    public KeyValuePair<bool, int> GetRandomGacha(string gachaType = "SINGLE")
    {
        float random = Random.Range(0.0f, 1.0f);
        int id;

        if(random < championProb)
        {//영웅
            id = championPool[Random.Range(0, championPool.Count)];
            if(SaveManager.Instance.championLevelDict[id].exp == 0
               && SaveManager.Instance.championLevelDict[id].level == 0)
            {
                SaveManager.Instance.championLevelDict[id].level = 1;
            }
            else
            {
                SaveManager.Instance.championLevelDict[id].exp += 1;
            }
            SaveManager.Instance.SaveChampionLevelData();
            if(CheckChampionExp(id) == false) championPool.Remove(id);
        }
        else
        {//타워
            id = towerPool[Random.Range(0, towerPool.Count)];
            if (SaveManager.Instance.towerLevelDict[id].exp == 0
                && SaveManager.Instance.towerLevelDict[id].level == 0)
            {
                SaveManager.Instance.towerLevelDict[id].level = 1;
            }
            else
            {
                SaveManager.Instance.towerLevelDict[id].exp += 1;
            }
            SaveManager.Instance.SaveTowerLevelData();
            if (CheckTowerExp(id) == false) towerPool.Remove(id);
        }
        
        bool isChampion = random < championProb;
        string resultName = isChampion
            ? DataManager.Instance.championDict[id].name
            : DataManager.Instance.towerDict[id].name;

        AnalyticsManager.SendEvent("GACHA_TOWER", new Dictionary<string, object>
        {
            { "PLAYER_HASGOLD", SaveManager.Instance.playerData.gold },
            { "PLAYER_CURRENTSTAGE", SaveManager.Instance.playerData.selectedStageIndex },
            { "GACHA_RESULT", resultName },
            { "NUMBEROFGACHA", gachaType }
        });
        
        return new KeyValuePair<bool, int>(random < championProb, id);
    }
}
