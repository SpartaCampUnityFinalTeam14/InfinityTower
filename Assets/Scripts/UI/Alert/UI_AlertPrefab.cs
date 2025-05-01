using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_AlertPrefab : Poolable
{
    [SerializeField] private Image alertBackground;
    [SerializeField] private TextMeshProUGUI alertText;

    private Color startImageColor = new(0f, 0f, 0f, 1f);
    private Color startTextColor = new(1f, 1f, 1f, 1f);
    private Vector3 startPos = Vector3.zero;
    private Vector3 endPos = Vector3.up * 200;
    private float duration = 2f;

    public void Init(string text)
    {
        alertText.text = text;

        alertBackground.color = startImageColor;
        alertText.color = startTextColor;
        transform.localPosition = startPos;

        alertBackground.DOFade(0f, duration).SetEase(Ease.Linear).SetUpdate(true);
        alertText.DOFade(0f, duration).SetEase(Ease.Linear).SetUpdate(true);
        alertBackground.transform.DOLocalMove(endPos, duration).SetEase(Ease.Linear).SetUpdate(true)
            .OnComplete(() => PoolManager.Instance.Release(this));
    }
}
