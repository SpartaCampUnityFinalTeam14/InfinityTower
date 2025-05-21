using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityManager
{
    // 특성 가챠에 사용될 특성 풀 (Dictionary<Rarity, Dictionary<특성ID, 특성데이터>>)
    public Dictionary<int, Dictionary<int, AbilityData>> abilityGachaPool { get; private set; }

    // 현재 스테이지에 적용 중인 특성 리스트 <특성id, 특성>
    public Dictionary<int, Ability> allAbilities { get; private set; } 

    private HashSet<int> hashSelectedTower;
    private AbilityHandler abilityHandler;

    public AbilityManager()
    {
        abilityGachaPool = new Dictionary<int, Dictionary<int, AbilityData>>();
        for (int i = 0; i < (int)Rarity.End; i++)
        {
            abilityGachaPool.Add(i, new Dictionary<int, AbilityData>());
        }

        hashSelectedTower = new HashSet<int>();
        allAbilities = new Dictionary<int, Ability>();
        abilityHandler = new AbilityHandler();

        FilterAbilitiesByDeck();
    }

    void FilterAbilitiesByDeck()
    {
        // HashSet Contains 시간 복잡도 O(1)
        hashSelectedTower.Clear();
        foreach (int TowerID in StageManager.Instance.selectedTowers)
        {
            hashSelectedTower.Add(TowerID);
        }

        foreach (var abilData in DataManager.Instance.abilityDict.Values)
        {
            // 특성 타입이 타워이고 선택된 타워 ID가 포함이 안될 때
            if (abilData.targetType.Equals((int)TargetType.Tower) && 
                abilData.targetID.Count > 0 &&
                !abilData.targetID.Any(id => hashSelectedTower.Contains(id)))
            {
                continue;
            }
            
            if (allAbilities.ContainsKey(abilData.perkID))
            {
                // 가지고 있는 특성의 경우 스택리밋보다 크면 추가 안함
                if (abilData.stackLimit <= allAbilities[abilData.perkID].CurStack)
                    return;
            }

            abilityGachaPool[(int)abilData.rarity].Add(abilData.perkID, abilData);
        }
    }

    public void AddAbillity(AbilityData data)
    {
        if (allAbilities.TryAdd(data.perkID, new Ability(data)))
        {
            // 추가된 특성 오브젝트에 적용
            abilityHandler.AddAbility(data.targetType, allAbilities[data.perkID]);
        }
        else
        {
            if (allAbilities[data.perkID].TryAddStack())
            {
                for (int i = 0; i < data.valueType.Count; i++)
                {
                    allAbilities[data.perkID].Data.value[i] += DataManager.Instance.abilityDict[data.perkID].value[i];
                }

                // 보유 특성 스택이 최대면 가챠 풀에서 제거
                RemoveAbilityInGachaPool(data);
            }
        }
    }

    public void RemoveAbility(AbilityData data)
    {
        if (allAbilities.ContainsKey(data.perkID))
        {
            // 특성 스택 제거
            allAbilities[data.perkID].SubStack(1);

            // 특성이 없을 때 삭제
            if (allAbilities[data.perkID].CurStack <= 0)
            {
                abilityHandler.RemoveAbility(data.targetType, allAbilities[data.perkID]);
                allAbilities.Remove(data.perkID);
            }

            // 제거한 특성이 가챠풀 안에 없으면 추가
            if (!abilityGachaPool[data.rarity].ContainsKey(data.perkID))
            {
                abilityGachaPool[data.rarity].Add(data.perkID, DataManager.Instance.abilityDict[data.perkID]);
            }
        }
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

    public AbilityData GetRandomAbility()
    {
        var abilityDatas = abilityGachaPool[GetRandomRarity()].Values.ToList();

        return abilityDatas[UnityEngine.Random.Range(0, abilityDatas.Count)];
    }

    public AbilityData GetRandomAbility(int rarity)
    {
        var abilityDatas = abilityGachaPool[rarity].Values.ToList();

        return abilityDatas.Count < 1 ? null : abilityDatas[UnityEngine.Random.Range(0, abilityDatas.Count)];
    }

    public void RemoveAbilityInGachaPool(AbilityData data)
    {
        if (DataManager.Instance.abilityDict[data.perkID].stackLimit <= allAbilities[data.perkID].CurStack)
            abilityGachaPool[data.rarity].Remove(data.perkID);
    }

    public List<Ability> GetAbilities(int type)
    {
        abilityHandler.TryGetAbilities(type, out List<Ability> list);

        return list;
    }
}
