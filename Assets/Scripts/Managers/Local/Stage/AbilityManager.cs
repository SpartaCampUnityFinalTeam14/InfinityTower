using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityManager
{
    public Dictionary<int, Dictionary<int, AbilityData>> FilterAbilityPool { get; private set; } // 특성 가챠에 사용될 특성 풀 (Dictionary<레어도, Dictionary<특성ID, 특성데이터>>)
    public Dictionary<int, Ability> CurAbilities { get; private set; } // 선택한 특성 리스트 <특성id, 특성>

    public Dictionary<int, float> monsterAbilities = new();

    public event Action<AbilityData> OnAddTowerAbility;
    public event Action<AbilityData> OnAddEnemyAbility;
    public event Action<AbilityData> OnAddCharacterAbility;
    public event Action<AbilityData> OnRemoveTowerAbility;
    public event Action<AbilityData> OnRemoveEnemyAbility;
    public event Action<AbilityData> OnRemoveCharacterAbility;

    public AbilityManager()
    {
        CurAbilities = new Dictionary<int, Ability>();

        FilterAbilitiesByDeck();
    }

    void UpdateMonsterAbility(AbilityData data)
    {
        var monsterAbilities = GetMonsterAbilities();
       
        for (int i = 0; i < data.valueType.Count; i++)
        {
            if (this.monsterAbilities.ContainsKey(data.valueType[i]))
            {
                this.monsterAbilities[data.valueType[i]] += DataManager.Instance.abilityDict[data.perkID].value[i]; ;
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

        foreach (var ability in CurAbilities.Values)
        {
            if (ability.Data.targetType == /*(int)TargetType.Enemy*/"enemy")
                listData.Add(ability);
        }
        
        return listData;
    }

    void FilterAbilitiesByDeck()
    {
        // Ditionary 초기화 작업
        FilterAbilityPool = new Dictionary<int, Dictionary<int, AbilityData>>();
        var abilityDatas = DataManager.Instance.abilityDict;
        foreach (var data in abilityDatas.Values)
        {
            if (!FilterAbilityPool.ContainsKey(data.rarity))
                FilterAbilityPool.Add(data.rarity, new Dictionary<int, AbilityData>());

            FilterAbilityPool[data.rarity].Add(data.perkID, data);
        }

        // 현재 덱에 관련된 특성만 남기기
        List<int> removeKey = new List<int>();
        foreach (var ability in FilterAbilityPool.Values)
        {
            removeKey.Clear();

            foreach (var data in ability.Values)
            {
                if (data.targetID != -1 && data.targetType.Equals((int)TargetType.Tower) 
                    && !StageManager.Instance.selectedTowers.Contains(data.targetID))
                    removeKey.Add(data.perkID);
            }

            foreach (var key in removeKey)
            {
                ability.Remove(key);
            }
        }
    }

    public void AddAbillity(AbilityData data)
    {
        if (!CurAbilities.TryAdd(data.perkID, new Ability(data)))
        {
            for (int i = 0; i < data.valueType.Count; i++)
            {
                CurAbilities[data.perkID].Data.value[i] += DataManager.Instance.abilityDict[data.perkID].value[i];
            }
        }

        // 특성 스택 증가
        CurAbilities[data.perkID].AddStackCount(1);

        // 보유 특성 스택이 최대면 가챠 풀에서 제거
        CheckStackable(data);

        // 추가된 특성 오브젝트에 적용
        if (data.targetType == "tower"/*(int)TargetType.Tower*/)
        {
            OnAddTowerAbility?.Invoke(data);
        }
        else if (data.targetType == "enemy"/*(int)TargetType.Enemy*/)
        {
            OnAddEnemyAbility?.Invoke(data);
        }
        else if (data.targetType == "character"/*(int)TargetType.Enemy*/)
        {
            OnAddCharacterAbility?.Invoke(data);
        }
    }

    public void RemoveAbility(AbilityData data)
    {
        if (CurAbilities.ContainsKey(data.perkID))
        {
            // 특성 스택 제거
            CurAbilities[data.perkID].SubStackCount(1);

            // 특성이 없을 때 삭제
            if (CurAbilities[data.perkID].CurStackCount <= 0)
            {
                CurAbilities.Remove(data.perkID);
            }

            // 제거한 특성이 가챠풀 안에 없으면 추가
            if (!FilterAbilityPool[data.rarity].ContainsKey(data.perkID))
            {
                FilterAbilityPool[data.rarity].Add(data.perkID, DataManager.Instance.abilityDict[data.perkID]);
            }

            // 제거된 특성 적용 해제
            if (data.targetType == "tower"/*(int)TargetType.Tower*/)
            {
                OnRemoveTowerAbility?.Invoke(data);
            }
            else if (data.targetType == "enemy"/*(int)TargetType.Enemy*/)
            {
                OnRemoveEnemyAbility?.Invoke(data);
            }
            else if (data.targetType == "character"/*(int)TargetType.Enemy*/)
            {
                OnRemoveCharacterAbility?.Invoke(data);
            }
        }
    }

    public List<Ability> GetAbilities(TowerData towerData)
    {
        List<Ability> listData = new();

        foreach (var ability in CurAbilities.Values)
        {
            if (ability.Data.targetType == "tower"/*(int)TargetType.Tower*/ && (ability.Data.targetID == -1 || ability.Data.targetID.Equals(towerData.id)))
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
        var abilityDatas = FilterAbilityPool[rarity].Values.ToList();

        return abilityDatas.Count < 1 ? null : abilityDatas[UnityEngine.Random.Range(0, abilityDatas.Count)];
    }

    public void CheckStackable(AbilityData data)
    {
        if (DataManager.Instance.abilityDict[data.perkID].stackLimit <= CurAbilities[data.perkID].CurStackCount)
            FilterAbilityPool[data.rarity].Remove(data.perkID);
    }
}
