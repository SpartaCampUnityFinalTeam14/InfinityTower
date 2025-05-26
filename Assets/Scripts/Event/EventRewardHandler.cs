using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class EventRewardHandler
{
    Dictionary<RewardType, Func<int, string>> rewardHandlers;

    public EventRewardHandler()
    {
        rewardHandlers = new Dictionary<RewardType, Func<int, string>>
        {
            { RewardType.RandomRarityPerk, value => AddRandomPerk(StageManager.Instance.abilityManager.GetRandomRarity(), value) },
            { RewardType.RandomCommonPerk, value => AddRandomPerk((int)Rarity.Common, value) },
            { RewardType.RandomRarePerk, value => AddRandomPerk((int)Rarity.Rare, value) },
            { RewardType.RandomEpicPerk, value => AddRandomPerk((int)Rarity.Epic, value) },
            { RewardType.Health, ApplyHealthReward },
            { RewardType.Cost, ApplyCostReward },
            { RewardType.Cooldown, ApplyCooldownReward }
        };
    }

    public string HandleReward(int type, int value)
    {
        if (rewardHandlers.TryGetValue((RewardType)type, out var handler))
        {
            return handler.Invoke(value);
        }

        return string.Empty;
    }

    string AddRandomPerk(int rarity, int count)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < count; i++)
        {
            var ability = StageManager.Instance.abilityManager.GetRandomAbility(rarity);
            
            if (ability == null)
                Debug.LogError("이벤트: 랜덤 특성 뽑기 실패");
            
            StageManager.Instance.abilityManager.AddAbillity(ability);
            
            sb.AppendLine($"{ability.name} 획득");
        }

        return sb.ToString().TrimEnd('\n');
    }

    string ApplyHealthReward(int value)
    {
        // 체력 증감 로직
        //value < 0 ? StageManager.Instance.TakeDamage(value) : StageManager.Instance.Heal(value);
        return $"체력 {value}";
    }

    string ApplyCostReward(int value)
    {
        // 코스트 보상 로직
        return "코스트 감소";
    }

    string ApplyCooldownReward(int value)
    {
        // 쿨타임 보상 로직
        return "쿨타임 감소";
    }
}
