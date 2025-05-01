using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class EventManager
{
    EventData eventData;
    EventData resultData;
    UIEvent uiEvent;
    EventRewardHandler rewardHandler;
    EventResultHandler resultHandler;

    public EventManager()
    {
        rewardHandler = new EventRewardHandler();
        resultHandler = new EventResultHandler();
    }

    EventData GetProbabilityEvent(int choiceID)
    {
        var probabilityData = DataManager.Instance.eventProbabilityDict[choiceID];
        float roll = Random.value;

        if (roll < probabilityData.drop1Per / 100f)
            return DataManager.Instance.eventResultDict[probabilityData.drop1ID];
        else
            return DataManager.Instance.eventResultDict[probabilityData.drop2ID];
    }

    public void ShowEvent()
    {
        var eventDict = DataManager.Instance.eventDict;
        eventData = eventDict[Random.Range(0, eventDict.Count)];

        uiEvent = UIManager.Instance.ShowUI<UIEvent>();
        uiEvent.SetEvent(eventData);
    }

    public void SelectChoice(int choiceIdx)
    {
        int[] arrID = new int[] { eventData.choice1ID, eventData.choice2ID, eventData.choice3ID };
        int choiceID = arrID[choiceIdx];

        // 이벤트 타입이 확률형이고 첫번째 선택지를 선택했을 경우, 첫번째 선택지가 무조건 확률 계산 이벤트
        if (eventData.type == (int)EventType.Probablity && choiceIdx == 0) 
        {
            resultData = GetProbabilityEvent(choiceID);

            // 랜덤으로 뽑힌 이벤트가 확률형 이벤트일 경우 다시 선택지 업데이트
            if (resultData.type == (int)EventType.Probablity) // 뽑기 성공
            {
                eventData = resultData;
                uiEvent.SetProbabilityEvent(eventData);
                return;
            }
        }
        else
        {
            // 나머지 이벤트
            resultData = DataManager.Instance.eventResultDict[choiceID];
        }

        // 보상 지급
        StringBuilder sbReward = new StringBuilder();
        for (int i = 0; i < resultData.rewardType.Count; i++)
        {
            sbReward.AppendLine(rewardHandler.HandleReward(resultData.rewardType[i], resultData.reward[i]));
        }

        // 결과 UI 업데이트
        uiEvent.SetResult(resultData, sbReward.ToString());
    }

    public void OnClickResultButton()
    {
        resultHandler.HandleResult(resultData);
    }
}
