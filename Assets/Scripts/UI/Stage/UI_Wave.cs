using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Wave : UI
{
    [SerializeField] Transform startPos;
    [SerializeField] Transform centerPos;
    [SerializeField] Transform endPos;
    [SerializeField] TextMeshProUGUI text;

    [Header("Wave Text Settings")]
    [SerializeField] float moveDuratrion;
    [SerializeField] float stopTime;
    [SerializeField] float exitDuration;

    [Header("Wave Clear Settings")]
    [SerializeField] float scaleDuration;

    public override void Show()
    {
        base.Show();

        StageManager.Instance.timeScaleManager.PushTimeScale(0f);
    }

    public override void Hide()
    {
        base.Hide();

        StageManager.Instance.timeScaleManager.PopTimeScale();
    }

    public void ShowWaveNum(int waveNum)
    {
        // Text Init
        text.text = $"WAVE {waveNum}";
        text.transform.position = startPos.transform.position;
        text.transform.localScale = Vector3.one;

        // 중앙 까지 moveDuratrion초 만큼 소모
        text.transform.DOMove(centerPos.transform.position, moveDuratrion).SetUpdate(true)
            .OnComplete(() =>
            {
                Sequence seq = DOTween.Sequence();
                seq.SetUpdate(true);

                // 중앙 도착 후 stopTime초 만큼 정지
                seq.AppendInterval(stopTime);
                // 퇴장까지 exitDuration초 소모
                seq.Append(text.transform.DOMove(endPos.transform.position, exitDuration))
                    .OnComplete(() => Hide());
            });
    }

    public void ShowWaveClear()
    {
        // Text Init
        text.text = $"Clear!";
        text.transform.position = centerPos.transform.position;
        text.transform.localScale = Vector3.zero;

        text.transform.DOScale(Vector3.one, scaleDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true)
            .OnComplete(() => Hide());
    }
}
