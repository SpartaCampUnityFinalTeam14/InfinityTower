using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFloorIntro : UI
{
    [SerializeField] List<RectTransform> pathPoint;
    [SerializeField] RectTransform player;
    [SerializeField] GameObject eventSign;

    int curFloor = 0;
    CanvasGroup canvasGroup;

    protected override void Awake()
    {
        base.Awake();

        player.transform.position = pathPoint[0].transform.position;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public override void Show()
    {
        base.Show();

        ShowFloorIntro();
    }

    public void ShowFloorIntro()
    {
        //StageManager.Instance.timeScaleManager.PushTimeScale(0f);

        canvasGroup.DOFade(1f, 0.5f);
        NextFloor();
    }

    void NextFloor()
    {
        curFloor++;

        player.DOMove(pathPoint[curFloor - 1].transform.position, 1f).SetUpdate(true);
        Invoke(nameof(CloseIntro), 2f);
    }

    void CloseIntro()
    {
        Hide();

        canvasGroup.DOFade(0f, 0.5f).SetUpdate(true);
        StageManager.Instance.isIntroEnd = true;
        //StageManager.Instance.timeScaleManager.PopTimeScale();
    }

    void CheckEvent()
    {
        if (curFloor == 3 || curFloor == 6)
        {
            eventSign.SetActive(true);
        }
    }
}
