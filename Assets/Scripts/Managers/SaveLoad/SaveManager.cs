using System.Collections.Generic;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    public PlayerData playerData;
    public Dictionary<int, TowerLevelData> towerLevelDict = new();
    public Dictionary<int, ChampionLevelData> championLevelDict = new();
    public Dictionary<int, ArtifactLevelData> artifactLevelDict = new();

    protected override void Awake()
    {
        base.Awake();

        Init();
    }

    public void Init()
    {
        LoadPlayerData();
        towerLevelDict = LoadJson<TowerLevelDataLoader, int, TowerLevelData>().MakeDict();
        championLevelDict = LoadJson<ChampionLevelDataLoader, int, ChampionLevelData>().MakeDict();
        artifactLevelDict = LoadJson<ArtifactLevelDataLoader, int, ArtifactLevelData>().MakeDict();
    }

    //public bool CheckDataFileExist(string className)
    //{
    //    string path = $"{Application.persistentDataPath}/{(nameof(PlayerData))}.json";
    //    return System.IO.File.Exists(path);
    //}

    Loader LoadJson<Loader, Key, Value>() where Loader : ISaveLoader<Key, Value>, new()
    {
        string path = $"{Application.persistentDataPath}/{typeof(Value)}.json";

        Loader loader;

        if (!System.IO.File.Exists(path))
        {
            Debug.LogWarning($"파일을 찾을 수 없습니다. 새로 생성합니다: {path}");
            loader = CreateNewLoader<Loader, Key, Value>();
            SaveDict<Loader, Key, Value>(loader.MakeDict());
            return loader;
        }

        // 기존 세이브 파일 로드
        try
        {
            string json = System.IO.File.ReadAllText(path);
            loader = JsonUtility.FromJson<Loader>(json);

            // 데이터 동기화 필요성 체크 및 실행
            if (SyncDataWithManager<Loader, Key, Value>(ref loader))
            {
                Debug.Log($"데이터가 동기화되었습니다: {typeof(Value)}");
                SaveDict<Loader, Key, Value>(loader.MakeDict());

            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"세이브 파일 로드 중 오류 발생: {e.Message}");
            Debug.LogWarning("새로운 세이브 파일을 생성합니다.");
            loader = CreateNewLoader<Loader, Key, Value>();
            SaveDict<Loader, Key, Value>(loader.MakeDict());
        }

        return loader;
    }

    Loader CreateNewLoader<Loader, Key, Value>() where Loader : ISaveLoader<Key, Value>, new()
    {
        Loader newLoader = new Loader();

        if (typeof(Value) == typeof(TowerLevelData))
        {
            List<Value> list = new();
            foreach (var tower in DataManager.Instance.towerDict)
            {
                if (tower.Value.id != tower.Value.originalID) continue;
                if (list.Count < 5)
                    list.Add((Value)(object)new TowerLevelData(tower.Key, 1, 0));
                else
                    list.Add((Value)(object)new TowerLevelData(tower.Key, 0, 0));
            }
            newLoader.data = list;
        }
        else if (typeof(Value) == typeof(ChampionLevelData))
        {
            List<Value> list = new();
            foreach (var champion in DataManager.Instance.championDict)
            {
                if (list.Count <= 0)
                    list.Add((Value)(object)new ChampionLevelData(champion.Key, 1, 0));
                else
                    list.Add((Value)(object)new ChampionLevelData(champion.Key, 0, 0));
            }
            newLoader.data = list;
        }
        else if (typeof(Value) == typeof(ArtifactLevelData))
        {
            List<Value> list = new();
            foreach (var artifactDict in DataManager.Instance.artifactDicts)
            {
                foreach (var artifact in artifactDict)
                {
                    list.Add((Value)(object)new ArtifactLevelData(artifact.Key, 0));
                }
            }
            newLoader.data = list;
        }

        return newLoader;
    }

    bool SyncDataWithManager<Loader, Key, Value>(ref Loader loader) where Loader : ISaveLoader<Key, Value>, new()
    {
        bool hasChanges = false;

        if (typeof(Value) == typeof(TowerLevelData))
        {
            hasChanges = SyncTowerData<Loader, Key, Value>(ref loader);
            if(hasChanges)
            {
                playerData.selectedTowerIndex = new() { -1, -1, -1, -1, -1 };
                SavePlayerData();
            }
        }
        else if (typeof(Value) == typeof(ChampionLevelData))
        {
            hasChanges = SyncChampionData<Loader, Key, Value>(ref loader);
            if (hasChanges)
            {
                playerData.selectedChampionIndex = 0;
                SavePlayerData();
            }
        }
        else if (typeof(Value) == typeof(ArtifactLevelData))
        {
            hasChanges = SyncArtifactData<Loader, Key, Value>(ref loader);
        }

        return hasChanges;
    }

    bool SyncTowerData<Loader, Key, Value>(ref Loader loader) where Loader : ISaveLoader<Key, Value>, new()
    {
        var saveDataDict = loader.MakeDict();
        var managerTowers = DataManager.Instance.towerDict;
        bool hasChanges = false;

        // DataManager에 있는 타워 중 세이브에 없는 것들 추가
        foreach (var tower in managerTowers)
        {
            if (tower.Value.id != tower.Value.originalID) continue;

            Key towerKey = (Key)(object)tower.Key;
            if (!saveDataDict.ContainsKey(towerKey))
            {
                var newTowerData = new TowerLevelData(tower.Key, 0, 0);
                loader.data.Add((Value)(object)newTowerData);
                hasChanges = true;
                Debug.Log($"새로운 타워 데이터 추가: {tower.Key}");
            }
        }

        // 세이브에 있지만 DataManager에 없는 타워들 제거
        var towersToRemove = new List<Value>();
        foreach (var saveData in loader.data)
        {
            var towerData = (TowerLevelData)(object)saveData;
            if (!managerTowers.ContainsKey(towerData.id))
            {
                towersToRemove.Add(saveData);
                hasChanges = true;
                Debug.Log($"제거된 타워 데이터 삭제: {towerData.id}");
            }
        }

        foreach (var towerToRemove in towersToRemove)
        {
            loader.data.Remove(towerToRemove);
        }

        loader.data.Sort((a, b) =>
        {
            var A = (TowerLevelData)(object)a;
            var B = (TowerLevelData)(object)b;
            return A.id.CompareTo(B.id);
        });

        return hasChanges;
    }

    // 챔피언 데이터 동기화
    bool SyncChampionData<Loader, Key, Value>(ref Loader loader) where Loader : ISaveLoader<Key, Value>, new()
    {
        var saveDataDict = loader.MakeDict();
        var managerChampions = DataManager.Instance.championDict;
        bool hasChanges = false;

        // DataManager에 있는 챔피언 중 세이브에 없는 것들 추가
        foreach (var champion in managerChampions)
        {
            Key championKey = (Key)(object)champion.Key;
            if (!saveDataDict.ContainsKey(championKey))
            {
                var newChampionData = new ChampionLevelData(champion.Key, 0, 0);
                loader.data.Add((Value)(object)newChampionData);
                hasChanges = true;
                Debug.Log($"새로운 챔피언 데이터 추가: {champion.Key}");
            }
        }

        // 세이브에 있지만 DataManager에 없는 챔피언들 제거
        var championsToRemove = new List<Value>();
        foreach (var saveData in loader.data)
        {
            var championData = (ChampionLevelData)(object)saveData;
            if (!managerChampions.ContainsKey(championData.id))
            {
                championsToRemove.Add(saveData);
                hasChanges = true;
                Debug.Log($"제거된 챔피언 데이터 삭제: {championData.id}");
            }
        }

        foreach (var championToRemove in championsToRemove)
        {
            loader.data.Remove(championToRemove);
        }

        loader.data.Sort((a, b) =>
        {
            var A = (ChampionLevelData)(object)a;
            var B = (ChampionLevelData)(object)b;
            return A.id.CompareTo(B.id);
        });

        return hasChanges;
    }

    // 아티팩트 데이터 동기화
    bool SyncArtifactData<Loader, Key, Value>(ref Loader loader) where Loader : ISaveLoader<Key, Value>, new()
    {
        var saveDataDict = loader.MakeDict();
        bool hasChanges = false;

        // DataManager의 모든 아티팩트 수집
        var allManagerArtifacts = new Dictionary<Key, bool>();
        foreach (var artifactDict in DataManager.Instance.artifactDicts)
        {
            foreach (var artifact in artifactDict)
            {
                Key artifactKey = (Key)(object)artifact.Key;
                allManagerArtifacts[artifactKey] = true;
            }
        }

        // DataManager에 있는 아티팩트 중 세이브에 없는 것들 추가
        foreach (var artifact in allManagerArtifacts)
        {
            if (!saveDataDict.ContainsKey(artifact.Key))
            {
                // Key를 다시 int로 변환해서 ArtifactLevelData 생성
                int artifactId = (int)(object)artifact.Key;
                var newArtifactData = new ArtifactLevelData(artifactId, 0);
                loader.data.Add((Value)(object)newArtifactData);
                hasChanges = true;
                Debug.Log($"새로운 아티팩트 데이터 추가: {artifact.Key}");
            }
        }

        // 세이브에 있지만 DataManager에 없는 아티팩트들 제거
        var artifactsToRemove = new List<Value>();
        foreach (var saveData in loader.data)
        {
            var artifactData = (ArtifactLevelData)(object)saveData;
            Key artifactKey = (Key)(object)artifactData.id;
            if (!allManagerArtifacts.ContainsKey(artifactKey))
            {
                artifactsToRemove.Add(saveData);
                hasChanges = true;
                Debug.Log($"제거된 아티팩트 데이터 삭제: {artifactData.id}");
            }
        }

        foreach (var artifactToRemove in artifactsToRemove)
        {
            loader.data.Remove(artifactToRemove);
        }

        loader.data.Sort((a, b) =>
        {
            var A = (ArtifactLevelData)(object)a;
            var B = (ArtifactLevelData)(object)b;
            return A.id.CompareTo(B.id);
        });

        return hasChanges;
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
        SaveDict<ArtifactLevelDataLoader, int, ArtifactLevelData>(artifactLevelDict);
    }

    public void SaveAll()
    {
        SavePlayerData();
        SaveTowerLevelData();
        SaveChampionLevelData();
        SaveArtifactSaveData();
    }

    [ContextMenu("ResetAllSaveFile")]
    public void ResetAll()
    {
        DeleteFile(nameof(PlayerData));
        DeleteFile(nameof(TowerLevelData));
        DeleteFile(nameof(ChampionLevelData));
        DeleteFile(nameof(ArtifactLevelData));

        Init();

        GameManager.Instance.isTutorialAlreadySeen = false;
        GameManager.Instance.LoadScene("KSM_Lobby");
    }

    void DeleteFile(string fileName)
    {
        string path = $"{Application.persistentDataPath}/{fileName}.json";

        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
            Debug.Log($"{fileName} 삭제 완료");
        }
    }
}
