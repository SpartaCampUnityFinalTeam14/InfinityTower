using System.Collections.Generic;
using UnityEngine;

public class SkillVisualDB : MonoBehaviour
{
    //public static SkillVisualDB Instance { get; private set; }

    private List<SkillVisualDataSO> visualDataList = new();
    private Dictionary<string, SkillVisualDataSO> visualDict = new();

    private void Awake()
    {
        //if (Instance != null)
        //{
        //    Destroy(gameObject);
        //    return;
        //}
        //Instance = this;

        // ✅ Resources 폴더에서 SkillVisualDataSO 전부 찾아서 등록
        visualDataList = new List<SkillVisualDataSO>(Resources.LoadAll<SkillVisualDataSO>("ScriptableObjects/SkillVisuals"));

        foreach (var so in visualDataList)
        {
            if (!visualDict.ContainsKey(so.id))
                visualDict.Add(so.id, so);
            else
                Debug.LogWarning($"⚠️ 중복된 SkillVisual ID 발견: {so.id}");
        }
    }

    public SkillVisualDataSO Get(string id)
    {
        if (!visualDict.TryGetValue(id, out var so))
        {
            Debug.LogWarning($"⚠️ SkillVisualDB: ID '{id}' 를 찾지 못했습니다.");
            throw new KeyNotFoundException($"SkillVisual ID '{id}' 없음");
        }
        else
        {
            Debug.Log($"✅ SkillVisualDB: ID '{id}' 에 해당하는 VisualData를 찾았습니다.");
        }

        return so;
    }
}