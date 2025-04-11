using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public Dictionary<int, AbilityData> abilities = new(); // 선택한 특성 리스트 <특성id, 특성>
    public event Action OnAbilityChanged;

    public void AddAbillity(AbilityData ability)
    {
        abilities.Add(ability.id, ability);
        OnAbilityChanged?.Invoke();
    }

    public List<AbilityData> GetAbilities(MonsterData monsterData)
    {
        List<AbilityData> listData = new();

        foreach (var data in abilities.Values)
        {
            if (data.targetType == (int)TargetType.Monster)
                listData.Add(data);
        }
        
        return listData;
    }

    public List<AbilityData> GetAbilities(TowerData towerData)
    {
        List<AbilityData> listData = new();

        foreach (var data in abilities.Values)
        {
            if (data.targetType == (int)TargetType.Tower && (data.targetID == -1 || data.targetID.Equals(towerData.id)))
                listData.Add(data);
        }

        return listData;
    }
}
