using UnityEngine;

[CreateAssetMenu(menuName = "Projectile/Projectile Data")]
public class ProjectileDataSO : ScriptableObject
{
    public string id;
    public GameObject prefab;

    public float speed;
    public float damage;

    public bool hasDoT;
    public float dotDuration;
    public float dotTickInterval;
    public float dotDamagePerTick;
    public float dotRadius;

    public bool hasSplash;
    public float splashRadius;

    public GameObject impactEffect;
    public AudioClip sound;
}
