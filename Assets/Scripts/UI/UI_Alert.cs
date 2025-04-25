using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Alert : UI
{
    [SerializeField] private Image alertBackground;
    [SerializeField] private TextMeshProUGUI alertText;

    private Vector3 startPos = Vector3.zero;
    private Vector3 endPos = Vector3.up * 200;
    private float duration = 2f;

    public void Init(string text)
    {
        alertText.text = text;

        Show();

        Color startColor = alertBackground.color;
        startColor.a = 1f;
        alertBackground.color = startColor;
        alertBackground.DOFade(0f, duration).SetEase(Ease.Linear).SetUpdate(true);

        alertBackground.transform.position = startPos;
        alertBackground.transform.DOLocalMove(endPos, duration).SetEase(Ease.Linear).SetUpdate(true)
            .OnComplete(() => Hide());
    }
}
