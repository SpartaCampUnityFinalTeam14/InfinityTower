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

    public Dictionary<int, float> monsterAbilities = new();

    private HashSet<int> hashSelectedTower;

    private AbilityHandler abilityHandler;
    public AbilityHandler AbilityHandle => abilityHandler;


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
        var abilDict = DataManager.Instance.abilityDict;

        hashSelectedTower.Clear();
        foreach (int TowerID in StageManager.Instance.selectedTowers)
        {
            hashSelectedTower.Add(TowerID);
        }

        bool result = false;
        foreach (var abilData in abilDict.Values)
        {
            // 특성 타겟이 타워가 아닐 때 or 타겟이 전체 적용일 경우 특성가챠 풀에 추가
            if (abilData.targetType != (int)TargetType.Tower || abilData.targetID.Count <= 0) 
                result = true;

            // 특성 타겟이 타워이고 전체 적용이 아닐 경우
            else if (abilData.targetID.Any(id => hashSelectedTower.Contains(id))) 
                result = true;
            
            if (result)
            {
                if (allAbilities.ContainsKey(abilData.perkID))
                {
                    // 현재 가지고 있는 특성의 스택이 최대값이 아닐 경우 가챠풀에 추가
                    if (allAbilities[abilData.perkID].CurStack < abilData.stackLimit)
                        abilityGachaPool[(int)abilData.rarity].Add(abilData.perkID, abilData);
                }
                else
                {
                    abilityGachaPool[(int)abilData.rarity].Add(abilData.perkID, abilData);
                }
            }
        }

        //// Ditionary 초기화 작업
        //FilterAbilityPool = new Dictionary<int, Dictionary<int, AbilityData>>();
        //var abilityDatas = DataManager.Instance.abilityDict;
        //foreach (var data in abilityDatas.Values)
        //{
        //    if (!FilterAbilityPool.ContainsKey(data.rarity))
        //        FilterAbilityPool.Add(data.rarity, new Dictionary<int, AbilityData>());

        //    FilterAbilityPool[data.rarity].Add(data.perkID, data);
        //}

        //// 현재 덱에 관련된 특성만 남기기
        //List<int> removeKey = new List<int>();
        //foreach (var ability in FilterAbilityPool.Values)
        //{
        //    removeKey.Clear();

        //    foreach (var data in ability.Values)
        //    {
        //        if (data.targetID != -1 && data.targetType.Equals((int)TargetType.Tower)
        //            && !StageManager.Instance.selectedTowers.Contains(data.targetID))
        //            removeKey.Add(data.perkID);
        //    }

        //    foreach (var key in removeKey)
        //    {
        //        ability.Remove(key);
        //    }
        //}
    }

    public void AddAbillity(AbilityData data)
    {
        if (!allAbilities.TryAdd(data.perkID, new Ability(data)))
        {
            for (int i = 0; i < data.valueType.Count; i++)
            {
                allAbilities[data.perkID].Data.value[i] += DataManager.Instance.abilityDict[data.perkID].value[i];
            }
        }

        // 특성 스택 증가
        allAbilities[data.perkID].AddStack(1);

        // 보유 특성 스택이 최대면 가챠 풀에서 제거
        CheckStackable(data);

        // 추가된 특성 오브젝트에 적용
        abilityHandler.ApplyAddAbility(data.targetType, allAbilities[data.perkID]);
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
                allAbilities.Remove(data.perkID);
            }

            // 제거한 특성이 가챠풀 안에 없으면 추가
            if (!abilityGachaPool[data.rarity].ContainsKey(data.perkID))
            {
                abilityGachaPool[data.rarity].Add(data.perkID, DataManager.Instance.abilityDict[data.perkID]);
            }

            // 제거된 특성 적용 해제
            abilityHandler.ApplyRemoveAbility(data.targetType, allAbilities[data.perkID]);
        }
    }

    public List<Ability> GetAbilities(TowerData towerData)
    {
        List<Ability> listData = new();

        foreach (var ability in allAbilities.Values)
        {
            if (ability.Data.targetType == (int)TargetType.Tower && (ability.Data.targetID.Count <= 0 || ability.Data.targetID.Contains(towerData.id)))
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

    public void CheckStackable(AbilityData data)
    {
        if (DataManager.Instance.abilityDict[data.perkID].stackLimit <= allAbilities[data.perkID].CurStack)
            abilityGachaPool[data.rarity].Remove(data.perkID);
    }
}
