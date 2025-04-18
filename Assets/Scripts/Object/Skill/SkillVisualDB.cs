using System.Collections.Generic;
using UnityEngine;

public class SkillVisualDB : MonoBehaviour
{
    public static SkillVisualDB Instance { get; private set; }

    [SerializeField]
    private List<SkillVisualDataSO> visualDataList = new();

    private Dictionary<string, SkillVisualDataSO> visualDict = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (var so in visualDataList)
        {
            if (!visualDict.ContainsKey(so.id))
                visualDict.Add(so.id, so);
        }
    }

    public SkillVisualDataSO Get(string id)
    {
        if (!visualDict.TryGetValue(id, out var so))
        {
            Debug.LogWarning($"⚠️ SkillVisualDB: ID '{id}' 를 찾지 못했습니다.");
        }
        else
        {
            Debug.Log($"✅ SkillVisualDB: ID '{id}' 에 해당하는 VisualData를 찾았습니다.");
        }

        return so;
    }

}