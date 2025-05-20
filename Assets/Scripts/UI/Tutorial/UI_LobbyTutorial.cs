using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_LobbyTutorial : UI
{
    public List<TutorialStep> steps = new();
    private TutorialStep curStep;

    [SerializeField] private RectTransform maskImage;
    [SerializeField] private List<RectTransform> maskPanels;
    [SerializeField] private TextMeshProUGUI explanationText;

    private void Start()
    {
        //StartStep();
    }

    public void StartStep()
    {
        curStep = FindTutorial(0);
        curStep.OnStep();
        explanationText.text = curStep.explanation;
    }

    public void NextStep(int curOrder)
    {
        curStep = FindTutorial(curOrder + 1);

        if (curStep == null)
        {
            Close();
            return;
        }

        curStep.OnStep();
        explanationText.text = curStep.explanation;
    }

    public void SetMaskPosition(RectTransform buttonRect)
    {
        Canvas.ForceUpdateCanvases();
        Canvas canvas = GetComponentInParent<Canvas>();

        Vector3[] corners = new Vector3[4];
        buttonRect.GetWorldCorners(corners);
        Vector2 screenBL = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[0]); // bottom-left
        Vector2 screenTR = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[2]); // top-right

        RectTransform canvasRT = canvas.transform as RectTransform;
        float canvasW = canvasRT.rect.width;
        float canvasH = canvasRT.rect.height;

        Vector2 localBL, localTR;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRT, screenBL, canvas.worldCamera, out localBL);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRT, screenTR, canvas.worldCamera, out localTR);

        ResetPanels();

        float topHeight = canvasH / 2f - localTR.y;
        float bottomHeight = localBL.y + canvasH / 2f;
        float leftWidth = localBL.x + canvasW / 2f;
        float rightWidth = canvasW / 2f - localTR.x;
        float tweenDuration = 0.5f;

        maskPanels[0].DOSizeDelta(new Vector2(maskPanels[0].sizeDelta.x, topHeight), tweenDuration).SetEase(Ease.InExpo);
        maskPanels[1].DOSizeDelta(new Vector2(leftWidth, maskPanels[2].sizeDelta.y), tweenDuration).SetEase(Ease.InExpo);
        maskPanels[2].DOSizeDelta(new Vector2(maskPanels[1].sizeDelta.x, bottomHeight), tweenDuration).SetEase(Ease.InExpo);
        maskPanels[3].DOSizeDelta(new Vector2(rightWidth, maskPanels[3].sizeDelta.y), tweenDuration).SetEase(Ease.InExpo);

        //RectTransform top = maskPanels[0];
        //float topHeight = canvasH / 2f - localTR.y;
        //top.sizeDelta = new Vector2(top.sizeDelta.x, topHeight);

        //RectTransform left = maskPanels[1];
        //float leftWidth = localBL.x + canvasW / 2f;
        //left.sizeDelta = new Vector2(leftWidth, left.sizeDelta.y);

        //RectTransform bottom = maskPanels[2];
        //float bottomHeight = localBL.y + canvasH / 2f;
        //bottom.sizeDelta = new Vector2(bottom.sizeDelta.x, bottomHeight);

        //RectTransform right = maskPanels[3];
        //float rightWidth = canvasW / 2f - localTR.x;
        //right.sizeDelta = new Vector2(rightWidth, right.sizeDelta.y);


        //Canvas.ForceUpdateCanvases();
        //Canvas canvas = GetComponentInParent<Canvas>();

        //// 2) 버튼의 월드 코너 4개 구하기
        //Vector3[] corners = new Vector3[4];
        //buttonRect.GetWorldCorners(corners);

        //// 3) 최소/최대 스크린 좌표 계산
        //Vector2 min = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[0]);
        //Vector2 max = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[2]);

        //// 4) 마스크 크기 및 위치 설정 (로컬 좌표로 변환)
        //Vector2 size = max - min;
        //Vector2 centerScreen = min + size * 0.5f;
        //Vector2 localPos;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //    canvas.transform as RectTransform,
        //    centerScreen, canvas.worldCamera, out localPos);

        //maskImage.anchoredPosition = localPos;
        //maskImage.sizeDelta = size;



        //maskImage.anchorMin = buttonRect.anchorMin;
        //maskImage.anchorMax = buttonRect.anchorMax;
        //maskImage.anchoredPosition = buttonRect.anchoredPosition;
        //maskImage.sizeDelta = buttonRect.sizeDelta;
        //maskImage.rotation = buttonRect.rotation;
        //maskImage.localScale = buttonRect.localScale;
        //maskImage.pivot = buttonRect.pivot;
    }

    void ResetPanels()
    {
        maskPanels[0].sizeDelta = new Vector2(0f, 0f); // Top
        maskPanels[1].sizeDelta = new Vector2(0f, 0f); // Bottom
        maskPanels[2].sizeDelta = new Vector2(0f, 0f); // Left
        maskPanels[3].sizeDelta = new Vector2(0f, 0f); // Right
    }

    public TutorialStep FindTutorial(int order)
    {
        foreach (var step in steps)
        {
            if (step.order == order) return step;
        }

        return null;
    }

    public override void Clear()
    {
        base.Clear();
    }
}
