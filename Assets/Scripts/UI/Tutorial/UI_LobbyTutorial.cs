using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_LobbyTutorial : UI
{
    public List<TutorialStep> steps = new();
    private TutorialStep curStep;

    [SerializeField] private RectTransform maskImage;
    [SerializeField] private TextMeshProUGUI explanationText;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        StartStep();
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
        maskImage.anchorMin = buttonRect.anchorMin;
        maskImage.anchorMax = buttonRect.anchorMax;
        maskImage.anchoredPosition = buttonRect.anchoredPosition;
        maskImage.sizeDelta = buttonRect.sizeDelta;
        maskImage.rotation = buttonRect.rotation;
        maskImage.localScale = buttonRect.localScale;
        maskImage.pivot = buttonRect.pivot;
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
