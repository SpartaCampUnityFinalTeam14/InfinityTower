using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EffectData
{
    public int id;
    public EffectType effectType;
    public int targetStatusID;
}

[Serializable]
public class EffectDataLoader : ILoader<int, EffectData>
{
    public List<EffectData> data = new();

    public Dictionary<int, EffectData> MakeDict()
    {
        Dictionary<int, EffectData> dict = new();
        foreach (EffectData tower in data)
        {
            dict.Add(tower.id, tower);
        }

        return dict;
    }
}
