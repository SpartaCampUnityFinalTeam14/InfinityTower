using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Visual Data")]
public class SkillVisualDataSO : ScriptableObject
{
    public string id;
    public GameObject effectPrefab;
    public GameObject explosionEffect;
    public AudioClip soundClip;
}

