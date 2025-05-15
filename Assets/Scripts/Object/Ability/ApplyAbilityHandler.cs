using System;
using System.Collections.Generic;

public class ApplyAbilityHandler
{
    Dictionary<string, Action<AbilityData>> addAbilityHandlers;
    Dictionary<string, Action<AbilityData>> removeAbilityHandlers;

    event Action<AbilityData> OnAddTowerAbility;
    event Action<AbilityData> OnAddEnemyAbility;
    event Action<AbilityData> OnAddCharacterAbility;

    event Action<AbilityData> OnRemoveTowerAbility;
    event Action<AbilityData> OnRemoveEnemyAbility;
    event Action<AbilityData> OnRemoveCharacterAbility;

    public ApplyAbilityHandler()
    {
        addAbilityHandlers = new Dictionary<string, Action<AbilityData>>
        {
            { "tower",  OnAddTowerAbility },
            { "enemy",  OnAddEnemyAbility },
            { "character",  OnAddCharacterAbility }
        };

        removeAbilityHandlers = new Dictionary<string, Action<AbilityData>>
        {
            { "tower",  OnRemoveTowerAbility },
            { "enemy",  OnRemoveEnemyAbility },
            { "character",  OnRemoveCharacterAbility }
        };
    }

    public void ApplyAddAbility(string type, AbilityData data)
    {
        if (addAbilityHandlers.TryGetValue(type, out var handle))
        {
            handle?.Invoke(data);
        }
    }

    public void ApplyRemoveAbility(string type, AbilityData data)
    {
        if (removeAbilityHandlers.TryGetValue(type, out var handle))
        {
            handle?.Invoke(data);
        }
    }

    public void ResisterAddAbilityEvent(string type, Action<AbilityData> action)
    {
        if (addAbilityHandlers.TryGetValue(type, out var handle))
        {
            handle += action;
        }
    }

    public void ResisterRemoveAbilityEvent(string type, Action<AbilityData> action)
    {
        if (removeAbilityHandlers.TryGetValue(type, out var handle))
        {
            handle += action;
        }
    }
}

