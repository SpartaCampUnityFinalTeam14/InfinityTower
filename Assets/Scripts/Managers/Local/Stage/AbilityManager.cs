using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public Dictionary<int, Ability> abilities = new(); // 선택한 특성 리스트 <특성id, 특성>
    public event Action OnAbilityChanged;

    public Dictionary<int, float> commonMonsterAbilities = new();

    void UpdateMonsterAbility(Ability ability)
    {
        var monsterAbilities = GetMonsterAbilities();
       
        for (int i = 0; i < ability.Data.valueType.Count; i++)
        {
            if (commonMonsterAbilities.ContainsKey(ability.Data.valueType[i]))
            {
                commonMonsterAbilities[ability.Data.valueType[i]] += ability.Data.value[i];
            }
            else
            {
                commonMonsterAbilities.Add(ability.Data.valueType[i], ability.Data.value[i]);
            }
        }
    }

    public void AddAbillity(Ability ability)
    {
        abilities.Add(ability.Data.id, ability);

        // 타겟타입으로 특성 업데이트 분류
        if (ability.Data.targetType == (int)TargetType.Tower)
        {
            OnAbilityChanged?.Invoke();
        }
        else if (ability.Data.targetType == (int)TargetType.Enemy)
        {
            UpdateMonsterAbility(ability);
        }
    }

    List<Ability> GetMonsterAbilities()
    {
        List<Ability> listData = new();

        foreach (var ability in abilities.Values)
        {
            if (ability.Data.targetType == (int)TargetType.Enemy)
                listData.Add(ability);
        }
        
        return listData;
    }

    public List<Ability> GetAbilities(TowerData towerData)
    {
        List<Ability> listData = new();

        foreach (var ability in abilities.Values)
        {
            if (ability.Data.targetType == (int)TargetType.Tower && (ability.Data.targetID == -1 || ability.Data.targetID.Equals(towerData.id)))
                listData.Add(ability);
        }

        return listData;
    }
}
