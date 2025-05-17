using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
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
    [SerializeField] float sequenceStartDelay;
    [SerializeField] float sequenceEndDelay;

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

    public void ShowWaveClear(Action onComplete = null)
    {
        // Text Init
        text.text = $"Clear!";
        text.transform.position = centerPos.transform.position;
        text.transform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true);

        // 시퀀스 시작 대기 시간
        seq.AppendInterval(sequenceStartDelay);

        // 시퀀스 동작
        seq.Append(text.transform.DOScale(Vector3.one, scaleDuration)
            .SetEase(Ease.OutBack).SetUpdate(true));

        // 클리어 문구 생성 후 끝나는 대기시간
        seq.AppendInterval(sequenceEndDelay);

        seq.AppendCallback(() =>
        {
            onComplete?.Invoke();
            Hide();
        });
    }
}
