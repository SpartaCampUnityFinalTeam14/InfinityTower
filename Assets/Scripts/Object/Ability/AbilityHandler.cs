using System;
using System.Collections;
using System.Collections.Generic;

public class AbilityHandler
{
    Dictionary<string, Action<AbilityData>> addAbilityHandlers;
    Dictionary<string, Action<AbilityData>> removeAbilityHandlers;

    event Action<AbilityData> OnAddTowerAbility;
    event Action<AbilityData> OnAddEnemyAbility;
    event Action<AbilityData> OnAddCharacterAbility;

    event Action<AbilityData> OnRemoveTowerAbility;
    event Action<AbilityData> OnRemoveEnemyAbility;
    event Action<AbilityData> OnRemoveCharacterAbility;

    // Case 1 Key: targetType / value: AbilityData
    Dictionary<string, List<Ability>> abilities;

    public AbilityHandler()
    {
        // legacy
        addAbilityHandlers = new Dictionary<string, Action<AbilityData>>
        {
            { "tower", OnAddTowerAbility },
            { "enemy", OnAddEnemyAbility },
            { "character", OnAddCharacterAbility }
        };

        removeAbilityHandlers = new Dictionary<string, Action<AbilityData>>
        {
            { "tower",  OnRemoveTowerAbility },
            { "enemy",  OnRemoveEnemyAbility },
            { "character",  OnRemoveCharacterAbility }
        };

        //// Update
        //Abilities = new Dictionary<string, List<Ability>>();
        //for (int i = 0; i < (int)TargetType.Enemy; i++)
        //{
        //    Abilities.Add($"{(TargetType)i}", new List<Ability>());
        //}
    }

    public void ApplyAddAbility(string type, AbilityData data)
    {
        // legacy
        if (addAbilityHandlers.TryGetValue(type, out var handle))
        {
            handle?.Invoke(data);
        }

        //// Update
        //if (Abilities.TryGetValue(type, out var handle2))
        //{
        //    handle2.Add(data);
        //}
    }

    public void ApplyRemoveAbility(string type, AbilityData data)
    {
        if (removeAbilityHandlers.TryGetValue(type, out var handle))
        {
            handle?.Invoke(data);
        }
    }

    public bool TryGetAbilities(string type, out List<Ability> list)
    {
        return abilities.TryGetValue(type, out list);
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

