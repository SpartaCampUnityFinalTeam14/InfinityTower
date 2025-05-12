using System.Collections.Generic;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    public PlayerData playerData;
    public Dictionary<int, TowerLevelData> towerLevelDict = new();
    public Dictionary<int, ChampionLevelData> championLevelDict = new();
    public Dictionary<int, ArtifactSaveData> artifactSaveDict = new();

    protected override void Awake()
    {
        base.Awake();

        LoadPlayerData();
        towerLevelDict = LoadJson<TowerLevelDataLoader, int, TowerLevelData>().MakeDict();
        championLevelDict = LoadJson<ChampionLevelDataLoader, int, ChampionLevelData>().MakeDict();
        artifactSaveDict = LoadJson<ArtifactSaveDataLoader, int, ArtifactSaveData>().MakeDict();
    }

    //public bool CheckDataFileExist(string className)
    //{
    //    string path = $"{Application.persistentDataPath}/{(nameof(PlayerData))}.json";
    //    return System.IO.File.Exists(path);
    //}

    Loader LoadJson<Loader, Key, Value>() where Loader : ISaveLoader<Key, Value>, new()
    {
        string path = $"{Application.persistentDataPath}/{typeof(Value)}.json";
        if (!System.IO.File.Exists(path))
        {
            Debug.LogWarning($"파일을 찾을 수 없습니다. 새로 생성합니다: {path}");
            Loader newLoader = new Loader();

            if(typeof(Value) == typeof(TowerLevelData))
            {
                List<Value> list = new();
                foreach (var tower in DataManager.Instance.towerDict)
                {
                    if(list.Count <= 0) list.Add((Value)(object)new TowerLevelData(tower.Key, 1, 0));
                    else list.Add((Value)(object)new TowerLevelData(tower.Key, 0, 0));
                }
                newLoader.data = list;
            }
            else if(typeof(Value) == typeof(ChampionLevelData))
            {
                List<Value> list = new();
                foreach (var champion in DataManager.Instance.championDict)
                {
                    if(list.Count <= 0) list.Add((Value)(object)new ChampionLevelData(champion.Key, 1, 0));
                    else list.Add((Value)(object)new ChampionLevelData(champion.Key, 0, 0));
                }
                newLoader.data = list;
            }
            else if(typeof(Value) == typeof(ArtifactSaveData))
            {
                List<Value> list = new();
                foreach(var artifactDict in DataManager.Instance.artifactDicts)
                {
                    foreach(var artifact in artifactDict)
                    {
                        list.Add((Value)(object)new ArtifactSaveData(artifact.Key, 0));
                    }
                }

                newLoader.data = list;
            }

            SaveDict<Loader, Key, Value>(newLoader.MakeDict());
            return newLoader;
        }

        string json = System.IO.File.ReadAllText(path);
        return JsonUtility.FromJson<Loader>(json);
    }

    public void SaveDict<Loader, Key, Value>(Dictionary<Key, Value> dict) where Loader : ISaveLoader<Key, Value>, new()
    {
        Loader loader = new Loader();
        loader.data = new List<Value>(dict.Values);

        string json = JsonUtility.ToJson(loader, true);

        string path = $"{Application.persistentDataPath}/{typeof(Value)}.json";
        System.IO.File.WriteAllText(path, json);

        Debug.Log($"{path} 저장 완료");
    }

    void LoadPlayerData()
    {
        string path = $"{Application.persistentDataPath}/{(nameof(PlayerData))}.json";
        if (!System.IO.File.Exists(path))
        {
            Debug.LogWarning($"캐릭터 파일을 찾을 수 없습니다. 새로 생성합니다: {path}");
            playerData = new();
            SavePlayerData();
            return;
        }

        string json = System.IO.File.ReadAllText(path);
        Debug.Log($"캐릭터 파일을 로드했습니다: {path}");
        playerData = JsonUtility.FromJson<PlayerData>(json);
        return;
    }

    public void SavePlayerData()
    {
        string json = JsonUtility.ToJson(playerData, true);
        string path = $"{Application.persistentDataPath}/{playerData.GetType()}.json";
        System.IO.File.WriteAllText(path, json);

        Debug.Log($"{path} 저장 완료");
    }

    public void SaveTowerLevelData()
    {
        SaveDict<TowerLevelDataLoader, int, TowerLevelData>(towerLevelDict);
    }

    public void SaveChampionLevelData()
    {
        SaveDict<ChampionLevelDataLoader, int, ChampionLevelData>(championLevelDict);
    }

    public void SaveArtifactSaveData()
    {
        SaveDict<ArtifactSaveDataLoader, int, ArtifactSaveData>(artifactSaveDict);
    }

    public void SaveAll()
    {
        SavePlayerData();
        SaveTowerLevelData();
        SaveChampionLevelData();
        SaveArtifactSaveData();
    }

    public void ClearAll()
    {
        playerData = new();
        SavePlayerData();
    }
}
