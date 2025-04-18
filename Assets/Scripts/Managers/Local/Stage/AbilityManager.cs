using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public Dictionary<int, Dictionary<int, AbilityData>> filterAbilityPool; // 특성 가챠에 사용될 특성 풀 (Dictionary<레어도, Dictionary<특성ID, 특성데이터>>)
    public Dictionary<int, Ability> abilities = new(); // 선택한 특성 리스트 <특성id, 특성>
    public event Action OnAbilityChanged;

    public Dictionary<int, float> monsterAbilities = new();

    private void Awake()
    {
        FilterAbilitiesByDeck();
    }

    void UpdateMonsterAbility(AbilityData data)
    {
        var monsterAbilities = GetMonsterAbilities();
       
        for (int i = 0; i < data.valueType.Count; i++)
        {
            if (this.monsterAbilities.ContainsKey(data.valueType[i]))
            {
                this.monsterAbilities[data.valueType[i]] += DataManager.Instance.abilityDict[data.id].value[i]; ;
            }
            else
            {
                this.monsterAbilities.Add(data.valueType[i], data.value[i]);
            }
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

    void FilterAbilitiesByDeck()
    {
        // Ditionary 초기화 작업
        filterAbilityPool = new Dictionary<int, Dictionary<int, AbilityData>>();
        var abilityDatas = DataManager.Instance.abilityDict;
        foreach (var data in abilityDatas.Values)
        {
            if (!filterAbilityPool.ContainsKey(data.rarity))
                filterAbilityPool.Add(data.rarity, new Dictionary<int, AbilityData>());

            filterAbilityPool[data.rarity].Add(data.id, data);
        }

        // 현재 덱에 관련된 특성만 남기기
        List<int> removeKey = new List<int>();
        foreach (var ability in filterAbilityPool.Values)
        {
            removeKey.Clear();

            foreach (var data in ability.Values)
            {
                if (data.targetID != -1 && data.targetType.Equals((int)TargetType.Tower) && !StageManager.Instance.selectedTowers.Contains(data.targetID))
                    removeKey.Add(data.id);
            }

            foreach (var key in removeKey)
            {
                ability.Remove(key);
            }
        }
    }

    public void AddAbillity(AbilityData data)
    {
        if (abilities.ContainsKey(data.id))
        {
            for (int i = 0; i < data.valueType.Count; i++)
            {
                abilities[data.id].Data.value[i] += DataManager.Instance.abilityDict[data.id].value[i];
            }
        }
        else
        {
            Ability ability = new Ability();
            ability.Init(data);
            abilities.Add(data.id, ability);
        }

        // 특성 스택 증가
        abilities[data.id].AddStackCount(1);

        // 특성 가챠 풀에서 스택형이 아니거나 최대 스택이면 제거
        CheckStackable(data);

        // 타겟타입으로 특성 업데이트 분류
        if (data.targetType == (int)TargetType.Tower)
        {
            OnAbilityChanged?.Invoke();
        }
        else if (data.targetType == (int)TargetType.Enemy)
        {
            UpdateMonsterAbility(data);
        }
    }

    public void RemoveAbility(AbilityData data)
    {
        if (abilities.ContainsKey(data.id))
        {
            abilities[data.id].SubStackCount(1);

            if (abilities[data.id].CurStackCount <= 0)
            {
                abilities.Remove(data.id);
                
                // 제거한 특성이 가챠풀 안에 없으면 추가
                if (!filterAbilityPool[data.rarity].ContainsKey(data.id))
                {
                    filterAbilityPool[data.rarity].Add(data.id, DataManager.Instance.abilityDict[data.id]);
                }
            }
        }
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

    public int GetRandomRarity()
    {
        // 1성 60%, 2성 30%, 3성 10%
        float roll = UnityEngine.Random.value;

        if (roll < 0.6f)
            return (int)Rarity.Common;
        else if (roll < 0.9f)
            return (int)Rarity.Rare;
        else
            return (int)Rarity.Epic;
    }

    public AbilityData GetRandomAbility(int rarity)
    {
        var abilityDatas = filterAbilityPool[rarity].Values.ToList();

        return abilityDatas.Count < 1 ? null : abilityDatas[UnityEngine.Random.Range(0, abilityDatas.Count)];
    }

    public void CheckStackable(AbilityData data)
    {
        if (DataManager.Instance.abilityDict[data.id].maxStack <= abilities[data.id].CurStackCount)
            filterAbilityPool[data.rarity].Remove(data.id);
    }
}
