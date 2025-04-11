using UnityEngine;

public class ChainLightningEffect : MonoBehaviour
{
    public float duration = 0.3f; // 이펙트 유지 시간

    private void Start()
    {
        Destroy(gameObject, duration);
    }
}