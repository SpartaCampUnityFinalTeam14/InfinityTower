using System.Collections.Generic;
using UnityEngine;

public class ArtifactGachaManager
{
    public int artifactCount;
    List<List<int>> artifactPool = new(3)
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
        foreach (var data in SaveManager.Instance.artifactSaveDict.Values)
        {
            artifactCount += data.count;

            if (data.count >= 3)
            {
                artifactPool[data.id / 1000].Remove(data.id);
            }
        }
    }

    public int GetRandomArtifact()
    {
        int rarity = GetRandomRarity();

        int randomId = Random.Range(0, artifactPool[rarity].Count);
        if (++SaveManager.Instance.artifactSaveDict[randomId].count >= 3) artifactPool[rarity].Remove(randomId);
        artifactCount++;
        SaveManager.Instance.SaveArtifactSaveData();
        return artifactPool[rarity][randomId];
    }

    int GetRandomRarity()
    {
        float prob = Random.Range(0f, 1f);

        if (prob < 0.6f) return 0;
        else if (prob < 0.9f) return 1;
        else return 2;
    }
}
