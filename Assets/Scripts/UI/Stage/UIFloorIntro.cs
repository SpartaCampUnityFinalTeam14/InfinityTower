using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFloorIntro : UI
{
    [SerializeField] RectTransform layerGroup;
    [SerializeField] RectTransform player;
    [SerializeField] GameObject eventSign;

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

        // 플로어 아이콘 생성
        var bossIcon = Instantiate(bossFloor, layerGroup);
        pathPoint.Add(bossIcon.GetComponent<RectTransform>());

        // 두 프레임 뒤에 플레이어 아이콘 위치 초기화
        StartCoroutine(InitPlayerPosition());
        canvasGroup.alpha = 0f;
    }

    public override void Show()
    {
        base.Show();

        Sequence sequence = DOTween.Sequence();
        sequence.Append(canvasGroup.DOFade(1f, 0.5f))
            .AppendCallback(NextFloor)
            .AppendInterval(2f)
            .AppendCallback(CloseIntro);
    }

    void NextFloor()
    {
        curFloor++;
        player.DOMove(pathPoint[curFloor - 1].transform.position, 1f).SetUpdate(true);
    }

    void CloseIntro()
    {
        Hide();

        canvasGroup.DOFade(0f, 0.5f).SetUpdate(true);
        StageManager.Instance.isIntroEnd = true;
    }

    IEnumerator InitPlayerPosition()
    {
        yield return null;
        yield return null;

        player.transform.position = pathPoint[0].transform.position;
    }

    void CheckEvent()
    {
        if (curFloor == 3 || curFloor == 6)
        {
            eventSign.SetActive(true);
        }
    }
}
