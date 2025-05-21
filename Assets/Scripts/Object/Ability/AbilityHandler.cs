using System;
using System.Collections;
using System.Collections.Generic;

public class AbilityHandler
{
    // Key: targetType / value: AbilityData
    Dictionary<int, List<Ability>> abilities;

    public AbilityHandler()
    {
        abilities = new Dictionary<int, List<Ability>>();
        for (int i = 0; i < (int)TargetType.End; i++)
        {
            abilities.TryAdd(i, new List<Ability>());
        }
    }

    public void AddAbility(int type, Ability data)
    {
        if (abilities.TryGetValue(type, out var handle))
        {
            handle.Add(data);

            // 타겟이 캐릭터인 경우 즉각 스테이지 매니져에 업데이트
            if (type.Equals(TargetType.Player))
            {
                foreach (var ability in handle)
                {
                    for (int i = 0; i < ability.Data.valueType.Count; i++)
                        StageManager.Instance.AddAbilityMultiplier(ability.Data.valueType[i], ability.Data.value[i]);
                }
            }
        }
    }

    public void RemoveAbility(int type, Ability data)
    {
        if (abilities.TryGetValue(type, out var handle))
        {
            handle.Remove(data);

            // 타겟이 캐릭터인 경우 즉각 스테이지 매니져에 업데이트
            if (type.Equals(TargetType.Player))
            {
                foreach (var ability in handle)
                {
                    for (int i = 0; i < ability.Data.valueType.Count; i++)
                        StageManager.Instance.RemoveAbilityMultiplier(ability.Data.valueType[i], ability.Data.value[i]);
                }
            }
        }
    }

    public bool TryGetAbilities(int type, out List<Ability> list)
    {
        if (abilities.TryGetValue(type, out var handle))
        {
            list = handle;
            return true;
        }

        list = null;
        return false;
    }
}

