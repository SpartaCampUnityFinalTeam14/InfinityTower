using UnityEngine;

public class ChainLightningEffect : MonoBehaviour
{
    public float duration = 0.3f;

    private void Start()
    {
        Destroy(this.gameObject, duration);
    }
}