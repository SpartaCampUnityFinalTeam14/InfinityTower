//캐릭터, 아이템 등의 초기값 로드 용도
using JetBrains.Annotations;
using System;
using System.Collections.Generic;

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
    public List<int> valueType;
    public List<float> value;
    public int enemyType;
    public bool hasSkill;
    public List<int> skillIds; // 스킬 ID만 저장
    public Dictionary<int, float> dictValue;
    public string description;

    public MonsterData(MonsterData data)
    {
        this.id = data.id;
        this.name = data.name;
        this.description = data.description;
        this.valueType = new List<int>(data.valueType);
        this.value = new List<float>(data.value);
        this.enemyType = data.enemyType;
        this.hasSkill = data.hasSkill;
        this.skillIds = new List<int>(data.skillIds);
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
    public int targettingRule;

    // 기본 스텟
    public List<int> statTypes;    // statType의 int 값들
    public List<float> statValue;   // 각 스탯에 대한 수치

    // 보유 효과
    public List<int> effectID;      // effctType의 int 값들

    // 각 효과의 수치, 지속시간, 중첩여부 (지속시간이 음수면 무한, 중첩여부가 0이하면 중첩안됨)
    public List<float[]> effectInfo;

    public TargettingRule TargettingRule => (TargettingRule)targettingRule;
    public TargetType TargetType => (TargetType)targetType;

    // 유틸 메서드: 특정 타입의 스탯 값 가져오기
    public float GetStatValue(StatType type)
    {
        for (int i = 0; i < statTypes.Count; i++)
        {
            if ((StatType)statTypes[i] == type)
                return statValue[i];
        }
        throw new InvalidOperationException($"{type.ToString()}에 해당하는 효과 없음");
    }

    public float GetStatValue(int typeID)
    {
        for (int i = 0; i < statTypes.Count; i++)
        {
            if (statTypes[i] == typeID)
                return statValue[i];
        }
        throw new InvalidOperationException($"ID:{typeID}에 해당하는 스탯 없음");
    }

    public string GetStatName(StatType type)
    {
        return DataManager.Instance.statusDict[(int)type].name;
    }

    public string GetStatName(int typeID)
    {
        return DataManager.Instance.statusDict[typeID].name;
    }

    // 유틸 메서드: 특정 타입의 스탯 값 가져오기
    public List<EffectBase> ReturnEffectList()
    {
        List<EffectBase> ret = new List<EffectBase>();

        for (int i = 0; i < effectID.Count; i++)
        {
            //이펙트 아이디를 타겟스테이터스 아이디로 변경
            int targetStatusID = DataManager.Instance.effectDict[effectID[i]].targetStatusID;
            float[] values = effectInfo[i];
            EffectBase effect = null;

            switch (targetStatusID)
            {
                case 0:
                    effect = new AttackDamageEffecter(targetStatusID);
                    break;

                case 1:
                    effect = new AttackRangeEffecter(targetStatusID);
                    break;

                case 2:
                    effect = new AttackSpeedEffecter(targetStatusID);
                    break;

                case 3:
                    effect = new TargetCountEffecter(targetStatusID);
                    break;

                case 4:
                    effect = new TowerCooldownEffecter(targetStatusID);
                    break;

                case 5:
                    effect = new CostEffecter(targetStatusID);
                    break;
            }

            ret.Add(effect);
        }
        return ret;
    }
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

#region ProjectileData
[Serializable]
public class ProjectileData
{
    public int id;
    public float speed;

    public bool hasDoT;
    public float dotDuration;
    public float dotTickInterval;
    public float dotDamagePerTick;
    public float dotRadius;

    public bool hasSplash;
    public float splashRadius;
}

[Serializable]
public class ProjectileDataLoader : ILoader<int, ProjectileData>
{
    public List<ProjectileData> data = new();

    public Dictionary<int, ProjectileData> MakeDict()
    {
        Dictionary<int, ProjectileData> dict = new();
        foreach (var item in data)
            dict.Add(item.id, item);
        return dict;
    }
}
#endregion


#region ChampionData
[Serializable]
public class ChampionData
{
    public int id;
    public string name;
    public List<int> skillId;
    public string description;
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
    public string name;
    public string description;
    public float coolTime;
    public float multiplier;
    public SkillType skillType;
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
    public int perkID;
    public string name;
    public string description;
    public List<int> valueType;
    public List<int> value;
    public List<int> effectType;
    public List<int> effectValue;
    public int projectile;
    public string targetType;
    public int targetID;
    public int rarity;
    public int stackLimit;
    public string image;

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
            dict.Add(ability.perkID, ability);
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
    public int type;
    public string title;
    public string description;
    public string choiceTitle;
    public string choice1;
    public string choice2;
    public string choice3;
    public int choice1ID;
    public int choice2ID;
    public int choice3ID;
    public List<int> rewardType;
    public List<int> reward;
    public string image;
    public string result;
}

[Serializable]
public class ProbabilityEventData
{
    public int id;
    public int drop1ID;
    public int drop1Per;
    public int drop2ID;
    public int drop2Per;
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

[Serializable]
public class ProbabilityEventDataLoader : ILoader<int, ProbabilityEventData>
{
    public List<ProbabilityEventData> data = new();

    public Dictionary<int, ProbabilityEventData> MakeDict()
    {
        Dictionary<int, ProbabilityEventData> dict = new();
        foreach (ProbabilityEventData eventData in data)
        {
            dict.Add(eventData.id, eventData);
        }

        return dict;
    }
}
#endregion

#region ArtifactData
[Serializable]
public class ArtifactData
{
    public int id;
    public string name;
    public int valueType;
    public int value;
    public float prob;
}

[Serializable]
public class ArtifactDataLoader : ILoader<int, ArtifactData>
{
    public List<ArtifactData> data = new();

    public Dictionary<int, ArtifactData> MakeDict()
    {
        Dictionary<int, ArtifactData> dict = new();
        foreach (ArtifactData artifact in data)
        {
            dict.Add(artifact.id, artifact);
        }

        return dict;
    }
}
#endregion

#region LevelUpData
[Serializable]
public class LevelUpData
{
    public int level;
    public int requiredExp;
    public float multiplier;
    public int remainedExp;
}

[Serializable]
public class LevelUpDataLoader : ILoader<int, LevelUpData>
{
    public List<LevelUpData> data = new();

    public Dictionary<int, LevelUpData> MakeDict()
    {
        Dictionary<int, LevelUpData> dict = new();
        foreach (LevelUpData levelUp in data)
        {
            dict.Add(levelUp.level, levelUp);
        }

        return dict;
    }
}
#endregion

#region StatusData
[Serializable]
public class StatusData
{
    public int id;
    public string type;
    public string name;
}

[Serializable]
public class StatusDataLoader : ILoader<int, StatusData>
{
    public List<StatusData> data = new();

    public Dictionary<int, StatusData> MakeDict()
    {
        Dictionary<int, StatusData> dict = new();
        foreach (StatusData status in data)
        {
            dict.Add(status.id, status);
        }

        return dict;
    }
}
#endregion