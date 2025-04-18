using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Visual Data")]
public class SkillVisualDataSO : ScriptableObject
{
    public string id;
    public GameObject effectPrefab;
    public GameObject explosionEffect;
    public AudioClip soundClip;

    private void OnEnable()
    {
        Debug.Log($"🧩 SO '{id}' 로드됨. 이펙트: {effectPrefab != null}");
    }
}

