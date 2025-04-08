using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public Dictionary<int, MonsterData> monsterDict = new();
    public Dictionary<int, FloorData> floorDict = new();
    public Dictionary<int, WaveData> waveDict = new();
    public Dictionary<int, TowerData> towerDict = new();
    public Dictionary<int, ChampionData> championDict = new();
    public Dictionary<int, SkillData> skillDict = new();
    public Dictionary<int, AbilityData> abilityDict = new();
    public Dictionary<int, AbilityType> abilityTypedict = new();

    protected override void Awake()
    {
        base.Awake();

        monsterDict = LoadJson<MonsterDataLoader, int, MonsterData>().MakeDict();
        floorDict = LoadJson<FloorDataLoader, int, FloorData>().MakeDict();
        waveDict = LoadJson<WaveDataLoader, int, WaveData>().MakeDict();
        towerDict = LoadJson<TowerDataLoader, int, TowerData>().MakeDict();
        championDict = LoadJson<ChampionDataLoader, int, ChampionData>().MakeDict();
        skillDict = LoadJson<SkillDataLoader, int, SkillData>().MakeDict();
        abilityDict = LoadJson<AbilityDataLoader, int, AbilityData>().MakeDict();
        abilityTypedict = LoadJson<AbilityTypeLoader, int, AbilityType>().MakeDict();
    }

    Loader LoadJson<Loader, Key, Value>() where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"Data/{typeof(Value)}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }
}
