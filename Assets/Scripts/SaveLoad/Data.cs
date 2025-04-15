//캐릭터, 아이템 등의 초기값 로드 용도
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

#region MonsterData
[Serializable]
public class MonsterData
{
    public int id;
    public string name;
    public string description;
    public List<int> valueType;
    public List<float> value;
    public int enemyType;
    public bool hasSkill;
    public float moveSpeed;

    public Dictionary<int, float> dictValue;

    public MonsterData(MonsterData data)
    {
        this.id = data.id;
        this.name = data.name;
        this.description = data.description;
        this.valueType = new List<int>(data.valueType);
        this.value = new List<float>(data.value);
        this.enemyType = data.enemyType;
        this.hasSkill = data.hasSkill;
        this.dictValue = new Dictionary<int, float>(data.dictValue);
    }
}

[Serializable]
public class MonsterDataLoader : ILoader<int, MonsterData>
{
    public List<MonsterData> data = new();

    public Dictionary<int, MonsterData> MakeDict()
    {
        Dictionary<int, MonsterData> dict = new();
        foreach (MonsterData monster in data)
        {
            dict.Add(monster.id, monster);

            monster.dictValue = new();
            for (int i = 0; i < monster.valueType.Count; i++)
            {
                monster.dictValue.Add(monster.valueType[i], monster.value[i]);
            }
        }


        return dict;
    }
}
#endregion

#region FloorData
[Serializable]
public class FloorData
{
    public int id;
    public int waveCount;
    public List<int> waveID;
    public List<float> spawnPosition;
}

[Serializable]
public class FloorDataLoader : ILoader<int, FloorData>
{
    public List<FloorData> data = new();

    public Dictionary<int, FloorData> MakeDict()
    {
        Dictionary<int, FloorData> dict = new();
        foreach (FloorData floor in data)
        {
            dict.Add(floor.id, floor);
        }

        return dict;
    }
}
#endregion

#region WaveData
[Serializable]
public class WaveData
{
    public int id;
    public List<int> enemyID;
    public List<int> spawnCount;
    public List<int> spawnDelayTime;
}

[Serializable]
public class WaveDataLoader : ILoader<int, WaveData>
{
    public List<WaveData> data = new();

    public Dictionary<int, WaveData> MakeDict()
    {
        Dictionary<int, WaveData> dict = new();
        foreach (WaveData wave in data)
        {
            dict.Add(wave.id, wave);
        }

        return dict;
    }
}
#endregion

#region TowerData


[Serializable]
public class TowerData
{
    public int id;
    public string name;
    public string description;
    public int targetType;
    public int targetCount;
    public int cost;
    public int targettingRule;
    public float value;
    public float coolTime;
    public float range;

    public TargettingRule TargettingRule => (TargettingRule)targettingRule;
    public TargetType TargetType => (TargetType)targetType;
}

[Serializable]
public class TowerDataLoader : ILoader<int, TowerData>
{
    public List<TowerData> data = new();

    public Dictionary<int, TowerData> MakeDict()
    {
        Dictionary<int, TowerData> dict = new();
        foreach (TowerData tower in data)
        {
            dict.Add(tower.id, tower);
        }

        return dict;
    }
}
#endregion

#region ChampionData
[Serializable]
public class ChampionData
{
    public int id;
    public List<int> skillID;
    public string desctiption;
    public int hp;
    public int atk;
}

[Serializable]
public class ChampionDataLoader : ILoader<int, ChampionData>
{
    public List<ChampionData> data = new();

    public Dictionary<int, ChampionData> MakeDict()
    {
        Dictionary<int, ChampionData> dict = new();
        foreach (ChampionData champion in data)
        {
            dict.Add(champion.id, champion);
        }

        return dict;
    }
}
#endregion

#region SkillData
[Serializable]
public class SkillData
{
    public int id;
    public string skillClassName;
    public string description;
    public float coolTime;
    public float multiplier;
    public float range;

    // 🔗 SO를 참조할 수 있는 필드 (Resources 또는 Addressable 기준 경로로 사용)
    public string visualId; // 예: "Meteor"
}


[Serializable]
public class SkillDataLoader : ILoader<int, SkillData>
{
    public List<SkillData> data = new();

    public Dictionary<int, SkillData> MakeDict()
    {
        Dictionary<int, SkillData> dict = new();
        foreach (SkillData skill in data)
        {
            dict.Add(skill.id, skill);
        }

        return dict;
    }
}
#endregion

#region AbilityData
[Serializable]
public class AbilityData
{
    public int id;
    public int rarity;
    public string name;
    public string description;
    public List<int> valueType;
    public List<int> value;
    public int targetType;
    public int targetID;
    public int stackable;
    public int maxStack;

    public AbilityData DeepCopy()
    {
        var copyData = (AbilityData)MemberwiseClone();
        copyData.valueType = new List<int>(valueType);
        copyData.value = new List<int>(value);

        return copyData;
    }
}

[Serializable]
public class AbilityDataLoader : ILoader<int, AbilityData>
{
    public List<AbilityData> data = new();

    public Dictionary<int, AbilityData> MakeDict()
    {
        Dictionary<int, AbilityData> dict = new();
        foreach (AbilityData ability in data)
        {
            dict.Add(ability.id, ability);
        }

        return dict;
    }
}
#endregion

#region AbilityType
[Serializable]
public class AbilityType
{
    public int id;
    public string type;
    public string description;
}

[Serializable]
public class AbilityTypeLoader : ILoader<int, AbilityType>
{
    public List<AbilityType> data = new();

    public Dictionary<int, AbilityType> MakeDict()
    {
        Dictionary<int, AbilityType> dict = new();
        foreach (AbilityType ability in data)
        {
            dict.Add(ability.id, ability);
        }

        return dict;
    }
}
#endregion

#region EventData
[Serializable]
public class EventData
{
    public int id;
    public string title;
    public int type;
    public string description;
    public string choiceTitle;
    public string choice1;
    public string choice2;
    public string choice3;
    public int choice1ID;
    public int choice2ID;
    public int choice3ID;
    public int rewardType;
    public int reward;
    public string image;
    public string buttonText;
}

[Serializable]
public class EventDataLoader : ILoader<int, EventData>
{
    public List<EventData> data = new();

    public Dictionary<int, EventData> MakeDict()
    {
        Dictionary<int, EventData> dict = new();
        foreach (EventData eventData in data)
        {
            dict.Add(eventData.id, eventData);
        }

        return dict;
    }
}
#endregion