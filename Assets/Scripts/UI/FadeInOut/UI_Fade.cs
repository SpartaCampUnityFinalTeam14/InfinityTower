using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Fade : UI
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float duration;
    [SerializeField] float fadeInterval;

    public void FadeOut(Action onComplete = null)
    {
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, duration).OnComplete(() =>
        {
            onComplete?.Invoke();
            //Hide();
        });
    }

    public void FadeIn(Action onComplete = null)
    {
        canvasGroup.alpha = 1f;

        StageManager.Instance.timeScaleManager?.PushTimeScale(0f);

        Sequence sequence = DOTween.Sequence();
        sequence.SetUpdate(true);

        sequence.AppendInterval(fadeInterval);
        sequence.Append(canvasGroup.DOFade(0f, duration)).OnComplete(() =>
        {
            StageManager.Instance.timeScaleManager.PopTimeScale();
            onComplete?.Invoke();
            Hide();
        });
    }
}
