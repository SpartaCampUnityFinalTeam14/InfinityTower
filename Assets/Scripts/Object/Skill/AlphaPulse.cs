using UnityEngine;

public class AlphaPulse : MonoBehaviour
{
    public float speed = 2f;
    public float minAlpha = 0.2f;
    public float maxAlpha = 0.8f;

    private SpriteRenderer sr;
    private float t;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogWarning("❌ AlphaPulse: SpriteRenderer 없음");
            enabled = false;
            return;
        }

        t = Random.Range(0f, 100f);
    }

    void Update()
    {
        if (sr == null) return;

        t += Time.deltaTime * speed;
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(t) + 1f) / 2f);

        Color c = sr.color;
        c.a = alpha;
        sr.color = c;
    }
}