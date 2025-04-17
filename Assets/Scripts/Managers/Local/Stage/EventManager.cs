using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    EventData eventData;
    EventData resultData;
    UIEvent uiEvent;
    EventRewardHandler rewardHandler;
    EventResultHandler resultHandler;

    private void Awake()
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
        uiEvent.SetEventPanel(eventData);
    }

    public void SelectChoice(int choiceIdx)
    {
        int[] arrID = new int[] { eventData.choice1ID, eventData.choice2ID, eventData.choice3ID };
        int choiceID = arrID[choiceIdx];

        // 선택지 이벤트 가져오기
        if (eventData.type == (int)EventType.Probablity && choiceIdx == 0)
        {
            resultData = GetProbabilityEvent(choiceID);
            eventData = resultData;
        }
        else
            resultData = DataManager.Instance.eventResultDict[choiceID];

        // 보상 지급
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < resultData.rewardType.Count; i++)
        {
            sb.AppendLine(rewardHandler.HandleReward(resultData.rewardType[i], resultData.reward[i]));
        }

        // UI 업데이트
        uiEvent.SetResultPanel(resultData);
        uiEvent.SetRewadText(sb.ToString());
        uiEvent.SetActiveResultPanel(true);
        uiEvent.EnableAllChoiceButton(false);
    }

    public void SetChoiceEvent(EventData data)
    {
        uiEvent.SetChoicePanel(data);
        uiEvent.SetActiveResultPanel(false);
        uiEvent.EnableAllChoiceButton(true);
    }

    public void OnClickResultButton()
    {
        resultHandler.HandleResult(resultData);
    }
}
