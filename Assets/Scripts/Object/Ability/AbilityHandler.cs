using System;
using System.Collections;
using System.Collections.Generic;

public class AbilityHandler
{
    //Dictionary<int, Action<AbilityData>> addAbilityHandlers;
    //Dictionary<int, Action<AbilityData>> removeAbilityHandlers;

    //event Action<AbilityData> OnAddTowerAbility;
    //event Action<AbilityData> OnAddEnemyAbility;
    //event Action<AbilityData> OnAddCharacterAbility;

    //event Action<AbilityData> OnRemoveTowerAbility;
    //event Action<AbilityData> OnRemoveEnemyAbility;
    //event Action<AbilityData> OnRemoveCharacterAbility;

    // Key: targetType / value: AbilityData
    Dictionary<int, List<Ability>> abilities;

    public AbilityHandler()
    {
        //// legacy
        //addAbilityHandlers = new Dictionary<int, Action<AbilityData>>
        //{
        //    { (int)TargetType.Tower, OnAddTowerAbility },
        //    { (int)TargetType.Enemy, OnAddEnemyAbility },
        //    { (int)TargetType.Player, OnAddCharacterAbility }
        //};

        //removeAbilityHandlers = new Dictionary<int, Action<AbilityData>>
        //{
        //    { (int)TargetType.Tower,  OnRemoveTowerAbility },
        //    { (int)TargetType.Enemy,  OnRemoveEnemyAbility },
        //    { (int)TargetType.Player,  OnRemoveCharacterAbility }
        //};

        // Update
        abilities = new Dictionary<int, List<Ability>>();
        for (int i = 0; i < (int)TargetType.End; i++)
        {
            abilities.TryAdd(i, new List<Ability>());
        }
    }

    public void ApplyAddAbility(int type, Ability data)
    {
        //// legacy
        //if (addAbilityHandlers.TryGetValue(type, out var handle))
        //{
        //    handle?.Invoke(data);
        //}

        // Update
        if (abilities.TryGetValue(type, out var handle2))
        {
            handle2.Add(data);
        }
    }

    public void ApplyRemoveAbility(int type, Ability data)
    {
        //if (removeAbilityHandlers.TryGetValue(type, out var handle))
        //{
        //    handle?.Invoke(data);
        //}
    }

    public bool TryGetAbilities(int type, out List<Ability> list)
    {
        return abilities.TryGetValue(type, out list);
    }

    //public void ResisterAddAbilityEvent(string type, Action<AbilityData> action)
    //{
    //    if (addAbilityHandlers.TryGetValue(type, out var handle))
    //    {
    //        handle += action;
    //    }
    //}

    //public void ResisterRemoveAbilityEvent(string type, Action<AbilityData> action)
    //{
    //    if (removeAbilityHandlers.TryGetValue(type, out var handle))
    //    {
    //        handle += action;
    //    }
    //}
}

