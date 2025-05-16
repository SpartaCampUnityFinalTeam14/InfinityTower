using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public TextMeshProUGUI text; // 또는 TMP_Text 로도 가능
    private float moveSpeed = 0.5f;
    private float fadeSpeed = 1f;

    private Color originColor;
    private bool isReady = false;

    private void Awake()
    {
        if (text == null)
            text = GetComponentInChildren<TextMeshProUGUI>();

        if (text == null)
        {
            Debug.LogError("❌ DamagePopup에서 TextMeshPro 컴포넌트를 찾지 못했습니다!");
            return;
        }

        originColor = text.color;
    }

    public void Setup(int damage)
    {
        if (text == null) return;

        text.text = damage.ToString();
        text.color = originColor;

        transform.position += new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(0f, 0.5f), 0f);

        isReady = true; // ✨ Set after setup
    }

    private void Update()
    {
        if (!isReady || text == null) return;

        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        Color c = text.color;
        c.a -= fadeSpeed * Time.deltaTime;
        text.color = c;

        if (c.a <= 0f)
        {
            Destroy(gameObject);
        }
    }
}