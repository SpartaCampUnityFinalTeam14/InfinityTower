using UnityEngine;

[CreateAssetMenu(menuName = "Projectile/Projectile Data")]
public class ProjectileDataSO : ScriptableObject
{
    public int id;
    public GameObject prefab;
    public GameObject impactEffect;
    public AudioClip sound;
}
