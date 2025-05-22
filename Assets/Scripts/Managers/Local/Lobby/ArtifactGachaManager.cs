using System;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactGachaManager
{
    private int artifactCount;
    private List<List<int>> artifactPool = new(3)
    {
        new List<int>(),
        new List<int>(),
        new List<int>(),
    };

    public ArtifactGachaManager()
    {
        CopyArtifactPool();

        RemoveAlreadyPulledArtifacts();
    }

    void CopyArtifactPool()
    {
        for(int i = 0; i < DataManager.Instance.artifactDicts.Count; i++)
        {
            foreach(var data in DataManager.Instance.artifactDicts[i])
            {
                artifactPool[i].Add(data.Key);
            }
        }
    }

    void RemoveAlreadyPulledArtifacts()
    {
        foreach (var data in SaveManager.Instance.artifactLevelDict.Values)
        {
            artifactCount += data.count;

            if (data.count >= 3)
            {
                artifactPool[data.id / 1000].Remove(data.id);
            }
        }
    }

    public bool IsAllArtifactPulled()
    {
        if (artifactCount >= SaveManager.Instance.artifactLevelDict.Count * 3) return true;
        else return false;
    }

    public int GetRandomArtifact()
    {
        int rarity = GetRandomRarity();

        if (artifactPool[rarity].Count <= 0)
        {//이미 전부 뽑힌 경우
            rarity = FindNonEmptyRarity(rarity);
        }

        int randomIndex = UnityEngine.Random.Range(0, artifactPool[rarity].Count);
        int artifactId = artifactPool[rarity][randomIndex];
        SaveManager.Instance.artifactLevelDict[artifactId].count++;
        if (SaveManager.Instance.artifactLevelDict[artifactId].count >= 3)
        {
            artifactPool[rarity].RemoveAt(randomIndex);
        }
        
        string artifactName = DataManager.Instance.artifactDicts[artifactId / 1000][artifactId].name;
        
        Debug.Log(artifactName);
        
        AnalyticsManager.SendEvent("GACHA_ARTIFACT", new Dictionary<string, object>
        {
            { "PLAYER_HASGOLD", SaveManager.Instance.playerData.gold },
            { "HAS_ARTIFACT", artifactCount },
            { "PLAYER_CURRENTSTAGE", SaveManager.Instance.playerData.selectedStageIndex },
            { "GACHA_RESULT_ARTIFACT", artifactName },
        });
        
        artifactCount++;
        SaveManager.Instance.SaveArtifactSaveData();
        return artifactId;
    }

    int FindNonEmptyRarity(int cur)
    {
        for(int i = cur; i < artifactPool.Count; i++)
        {
            if(artifactPool[i].Count > 0) return i;
        }
        for (int i = cur; i >= 0; i--)
        {
            if (artifactPool[i].Count > 0) return i;
        }

        throw new InvalidOperationException($"모든 유물이 전부 다 뽑혀서 뽑을 수 있는 유물이 없습니다.");
    }

    int GetRandomRarity()
    {
        float prob = UnityEngine.Random.Range(0f, 1f);

        if (prob < 0.6f) return 0;
        else if (prob < 0.9f) return 1;
        else return 2;
    }
}
