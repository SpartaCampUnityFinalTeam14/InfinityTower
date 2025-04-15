using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    EventData eventData;
    EventData resultData;
    UIEvent uiEvent;

    public void ShowEvent()
    {
        var eventDict = DataManager.Instance.eventDict;
        eventData = eventDict[Random.Range(0, eventDict.Count)];

        SetEvent();
    }

    void SetEvent()
    {
        uiEvent = UIManager.Instance.ShowUI<UIEvent>();
        uiEvent.SetEventText(eventData);
    }

    void GetReward()
    {
        string rewardName = string.Empty;
        AbilityData ability;

        switch (resultData.rewardType)
        {
            case (int)RewardType.RandomRarityPerk:
                int rand = StageManager.Instance.abilityManager.GetRandomRarity();
                ability = StageManager.Instance.abilityManager.GetRandomAbility(rand);
                StageManager.Instance.abilityManager.AddAbillity(ability);
                rewardName = ability.name;
                break;
            case (int)RewardType.RandomCommonPerk:
                ability = StageManager.Instance.abilityManager.GetRandomAbility((int)Rarity.Common);
                StageManager.Instance.abilityManager.AddAbillity(ability);
                rewardName = ability.name;
                break;
            case (int)RewardType.RandomRarePerk:
                ability = StageManager.Instance.abilityManager.GetRandomAbility((int)Rarity.Rare);
                StageManager.Instance.abilityManager.AddAbillity(ability);
                rewardName = ability.name;
                break;
            case (int)RewardType.RandomEpicPerk:
                ability = StageManager.Instance.abilityManager.GetRandomAbility((int)Rarity.Epic);
                StageManager.Instance.abilityManager.AddAbillity(ability);
                rewardName = ability.name;
                break;
            case (int)RewardType.Health:
                break;
            case (int)RewardType.Cost:
                break;
            case (int)RewardType.Cooldown:
                break;
        }

        uiEvent.SetRewadText(rewardName);
    }

    public void SelectChoice(int choiceIdx)
    {
        int[] arrID = new int[] { eventData.choice1ID, eventData.choice2ID, eventData.choice3ID };
        int choiceID = arrID[choiceIdx];

        resultData = DataManager.Instance.eventResultDict[choiceID];

        GetReward();

        uiEvent.SetResultText(resultData);
        uiEvent.SetActiveResult(true);
        uiEvent.DisableAllChoiceButton();
    }

    public void OnClickResultButton()
    {
        switch (resultData.type) 
        {
            case (int)EventType.Battle:
                StageManager.Instance.AddFloorCount(1);
                uiEvent.Hide();
                break;
            case (int)EventType.Penalty:
            case (int)EventType.ReturnStage:
                uiEvent.Hide();
                break;
        }
    }
}
