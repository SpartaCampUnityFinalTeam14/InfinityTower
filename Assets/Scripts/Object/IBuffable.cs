using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuffable
{
    Dictionary<int, float> AddModifierStat { get; set; }
    List<int> ValidStatTypes { get; }
}

public static class ArtifactHelper
{
    public static void ApplyArtifactModifiers(IBuffable target)
    {
        foreach (var artifactData in SaveManager.Instance.artifactLevelDict)
        {
            int statType = (int)artifactData.Value.ReturnMyStatType();

            if (!target.ValidStatTypes.Contains(statType))
                continue;

            float value = artifactData.Value.ReturnNowStatValue((StatType)statType);
            if (!target.AddModifierStat.TryAdd(statType, value))
            {
                target.AddModifierStat[statType] += value;
            }
        }
    }
}
