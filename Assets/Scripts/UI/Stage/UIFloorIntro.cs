using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFloorIntro : UI
{
    [SerializeField] RectTransform layerGroup;
    [SerializeField] RectTransform player;
    [SerializeField] GameObject eventSign;
    [SerializeField] LayoutGroup layoutGourp;

    [Header("Intro Settings")]
    [SerializeField] float fadeDuration;

    bool isFirstShow;
    int curFloor = 0;
    CanvasGroup canvasGroup;
    List<RectTransform> pathPoint;

    protected override void Awake()
    {
        base.Awake();

        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Init(int floorCnt)
    {
        pathPoint = new List<RectTransform>();

        GameObject floor = Resources.Load<GameObject>("Prefabs/UI/FloorIntro/FloorIcon");
        GameObject iconEvent = Resources.Load<GameObject>("Prefabs/UI/FloorIntro/EventIcon");
        GameObject bossFloor = Resources.Load<GameObject>("Prefabs/UI/FloorIntro/FloorIcon");

        for (int i = 0; i < floorCnt; i++) 
        {
            int floorNum = i + 1;

            // 플로어 아이콘 생성
            var floorIcon = Instantiate(floor, layerGroup);
            pathPoint.Add(floorIcon.GetComponent<RectTransform>());

            // 짝수 플로어일 경우 이벤트 아이콘 생성
            if (floorNum != 0 && floorNum % 2 == 0)
            {
                var evnetIcon = Instantiate(iconEvent, layerGroup);
                pathPoint.Add(evnetIcon.GetComponent<RectTransform>());
            }
        }

        // 보스플로어 아이콘 생성
        var bossIcon = Instantiate(bossFloor, layerGroup);
        pathPoint.Add(bossIcon.GetComponent<RectTransform>());

        isFirstShow = true;
        canvasGroup.alpha = 0f;
    }

    public override void Show()
    {
        base.Show();

        StageManager.Instance.timeScaleManager.PushTimeScale(0f);

        // 플레이어 아이콘 위치 잡아주기
        if (isFirstShow)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGourp.GetComponent<RectTransform>());
            player.transform.position = pathPoint[0].transform.position;
            isFirstShow = false;
        }

        Sequence sequence = DOTween.Sequence();
        sequence.SetUpdate(true);

        sequence.Append(canvasGroup.DOFade(1f, fadeDuration))
            .AppendCallback(NextFloor)
            .AppendInterval(2f)
            .Append(canvasGroup.DOFade(0f, fadeDuration))
            .OnComplete(() =>
            {
                CloseIntro();
            });
    }

    void NextFloor()
    {
        curFloor++;
        player.DOMove(pathPoint[curFloor].transform.position, 1f).SetUpdate(true);
    }

    void CloseIntro()
    {
        StageManager.Instance.timeScaleManager.PopTimeScale();
        StageManager.Instance.isIntroEnd = true;
        Hide();
    }
}
