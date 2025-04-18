using System.Collections.Generic;
using System.Linq;
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
    public Dictionary<int, EventData> eventDict = new();
    public Dictionary<int, EventData> eventResultDict = new();
    public Dictionary<int, ProbabilityEventData> eventProbabilityDict = new();
    public List<Dictionary<int, ArtifactData>> artifactDicts = new(3)
    {
        new Dictionary<int, ArtifactData>(),
        new Dictionary<int, ArtifactData>(),
        new Dictionary<int, ArtifactData>()
    };

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
        eventDict = LoadJson<EventDataLoader, int, EventData>().MakeDict();
        eventResultDict = LoadJson<EventDataLoader, int, EventData>("EventResultData").MakeDict();
        eventProbabilityDict = LoadJson<ProbabilityEventDataLoader, int, ProbabilityEventData>("EventProbabilityData").MakeDict();
        artifactDicts[0] = LoadJson<ArtifactDataLoader, int, ArtifactData>("Artifact_Common").MakeDict();
        artifactDicts[1] = LoadJson<ArtifactDataLoader, int, ArtifactData>("Artifact_Rare").MakeDict();
        artifactDicts[2] = LoadJson<ArtifactDataLoader, int, ArtifactData>("Artifact_Epic").MakeDict();
    }

    Loader LoadJson<Loader, Key, Value>(string fileName = default) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Resources.Load<TextAsset>(string.IsNullOrEmpty(fileName) ? $"Data/{typeof(Value)}" : $"Data/{fileName}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }
}
