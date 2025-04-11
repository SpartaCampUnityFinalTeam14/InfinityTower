using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public Dictionary<int, AbilityData> abilities = new(); // 선택한 특성 리스트 <특성id, 특성>
    public event Action OnAbilityChanged;

    public Dictionary<int, float> commonMonsterAbilities = new();

    void UpdateMonsterAbility(AbilityData ability)
    {
        var monsterAbilities = GetMonsterAbilities();
       
        for (int i = 0; i < ability.valueType.Count; i++)
        {
            if (commonMonsterAbilities.ContainsKey(ability.valueType[i]))
            {
                commonMonsterAbilities[ability.valueType[i]] += ability.value[i];
            }
            else
            {
                commonMonsterAbilities.Add(ability.valueType[i], ability.value[i]);
            }
        }
    }

    public void AddAbillity(AbilityData ability)
    {
        abilities.Add(ability.id, ability);

        // 타겟타입으로 특성 업데이트 분류
        if (ability.targetType == (int)TargetType.Tower)
        {
            OnAbilityChanged?.Invoke();
        }
        else if (ability.targetType == (int)TargetType.Monster)
        {
            UpdateMonsterAbility(ability);
        }
    }

    List<AbilityData> GetMonsterAbilities()
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
