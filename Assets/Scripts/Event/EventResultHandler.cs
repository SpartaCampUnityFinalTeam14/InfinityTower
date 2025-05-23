using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventResultHandler
{
    Dictionary<EventType, Action<EventData>> resultHandlers;

    public EventResultHandler()
    {
        resultHandlers = new Dictionary<EventType, Action<EventData>>
        {
            { EventType.Battle, HandleBattle},
            //{ EventType.Probablity, HandleProbabilty},
            { EventType.PerkChange, HandlePerkChange }
        };
    }

    public void HandleResult(EventData data)
    {
        if (resultHandlers.TryGetValue((EventType)data.type, out var handle))
            handle.Invoke(data);
        else
            HandleDefault(data);
    }

    void HandleBattle(EventData data)
    {
        StageManager.Instance.isAdditionalFloor = true;
        
        var ui = UIManager.Instance.GetUI<UIEvent>();
        ui.CloseEvent();
    }

    //void HandleProbabilty(EventData data)
    //{
    //    StageManager.Instance.eventManager.SetChoiceEvent(data);
    //}

    void HandlePerkChange(EventData data)
    {
        UIManager.Instance.ShowUI<UIFortuneEvent>();
    }

    void HandleDefault(EventData data)
    {
        var ui = UIManager.Instance.GetUI<UIEvent>();
        ui.CloseEvent();
    }
}
