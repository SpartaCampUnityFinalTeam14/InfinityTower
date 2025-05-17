using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class UI_FloorLoading : UI
{
    [Header("Setting")]
    [SerializeField] float openShopDelay;
    [SerializeField] float startDelay;
    [SerializeField] float endDelay;
    [SerializeField] float moveDuration;
    [SerializeField] float fadeInDuration;
    [SerializeField] CanvasGroup canvasGroup;

    [Header("MovePath")]
    [SerializeField] RectTransform Player;
    [SerializeField] RectTransform startPos;
    [SerializeField] RectTransform middlePos;
    [SerializeField] RectTransform endPos;

    public override void Show()
    {
        base.Show();

        StageManager.Instance.timeScaleManager.PushTimeScale(0f);

        canvasGroup.alpha = 1f;
        Player.position = startPos.position;

        StartCoroutine(DelayOpenShop());
    }

    public void SequenceStart()
    {
        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true);

        seq.AppendInterval(startDelay);

        seq.Append(Player.DOMove(middlePos.position, moveDuration).SetEase(Ease.Linear));
        //seq.AppendInterval(moveDuration);

        seq.Append(Player.DOMove(endPos.position, moveDuration).SetEase(Ease.Linear));
        seq.AppendInterval(moveDuration + endDelay);

        seq.Append(canvasGroup.DOFade(0f, fadeInDuration).OnComplete(() =>
        {
            Hide();

            StageManager.Instance.isIntroEnd = true;
            StageManager.Instance.timeScaleManager.PopTimeScale();
        }));

    }

    IEnumerator DelayOpenShop()
    {
        yield return new WaitForSecondsRealtime(openShopDelay);

        UIManager.Instance.ShowUI<UI_Shop>();
    }
}
