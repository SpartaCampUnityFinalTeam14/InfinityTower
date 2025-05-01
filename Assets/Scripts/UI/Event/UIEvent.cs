using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIEvent : UI
{
    [Header("PanelPositions")]
    [SerializeField] RectTransform firstPanelPos;
    [SerializeField] RectTransform secondPanelPos;

    [Header ("Event Panel")]
    [SerializeField] GameObject pageMain;
    [SerializeField] GameObject pageChoice;
    [SerializeField] GameObject pageResult;
    [SerializeField] GameObject pageReward;

    [Header("MainEvent")]
    [SerializeField] TextMeshProUGUI eventTitle;
    [SerializeField] TextMeshProUGUI eventDesc;
    //[SerializeField] Button btnNext;
    [SerializeField] Image image;

    [Header ("ChoiceEvent")]
    [SerializeField] TextMeshProUGUI choiceTitle;
    [SerializeField] Button btnChoice1;
    [SerializeField] Button btnChoice2;
    [SerializeField] Button btnChoice3;

    [Header ("ResultEvent")]
    [SerializeField] TextMeshProUGUI resultTitle;
    [SerializeField] TextMeshProUGUI resultDesc;
    [SerializeField] TextMeshProUGUI resultReward;
    [SerializeField] Button btnResult;
    protected override void Awake()
    {
        base.Awake();

        btnChoice1.onClick.AddListener(() => StageManager.Instance.eventManager.SelectChoice(0));
        btnChoice2.onClick.AddListener(() => StageManager.Instance.eventManager.SelectChoice(1));
        btnChoice3.onClick.AddListener(() => StageManager.Instance.eventManager.SelectChoice(2));
        btnResult.onClick.AddListener(StageManager.Instance.eventManager.OnClickResultButton);
    }

    public override void Show()
    {
        base.Show();

        StageManager.Instance.timeScaleManager.PushTimeScale(0f);
    }

    public override void Hide()
    {
        base.Hide();

        StageManager.Instance.timeScaleManager.PopTimeScale();
        StageManager.Instance.isEventEnd = true;
    }

    public void SetEvent(EventData data)
    {
        // 메인 이벤트 UI 설정
        eventTitle.text = data.title;
        eventDesc.text = data.description;

        var sprite = Resources.Load<Sprite>($"Event/{data.image}");
        if (sprite) image.sprite = sprite;

        // 이벤트 선택지 설정
        UpdateChoicePanel(data);

        SetPanelsPostion(pageMain, pageChoice);
    }

    public void UpdateChoicePanel(EventData data)
    {
        choiceTitle.text = data.choiceTitle;

        ClearAllChoiceButton();
        if (!string.IsNullOrEmpty(data.choice1))
        {
            btnChoice1.GetComponentInChildren<TextMeshProUGUI>().text = data.choice1;
            //btnChoice1.enabled = true;
            btnChoice1.gameObject.SetActive(true);
        }

        if (!string.IsNullOrEmpty(data.choice2))
        {
            btnChoice2.GetComponentInChildren<TextMeshProUGUI>().text = data.choice2;
            //btnChoice2.enabled = true;
            btnChoice2.gameObject.SetActive(true);
        }

        if (!string.IsNullOrEmpty(data.choice3))
        {
            btnChoice3.GetComponentInChildren<TextMeshProUGUI>().text = data.choice3;
            //btnChoice3.enabled = true;
            btnChoice3.gameObject.SetActive(true);
        }
    }

    public void SetResult(EventData resultEvent, string reward)
    {
        // Update ResultPanel
        resultTitle.text = resultEvent.title;
        resultDesc.text = resultEvent.description;

        // Update RewardPanel
        resultReward.text = string.IsNullOrEmpty(reward) ? "" : reward;
        btnResult.GetComponentInChildren<TextMeshProUGUI>().text = string.IsNullOrEmpty(resultEvent.result) ? "Event Close" : resultEvent.result;

        // Update Panel Postion
        SetPanelsPostion(pageResult, pageReward);
    }

    public void SetProbabilityEvent(EventData data)
    {
        // Update ResultPanel
        resultTitle.text = data.title;
        resultDesc.text = data.description;
        
        UpdateChoicePanel(data);

        // Update Panel Position
        SetPanelsPostion(pageResult, pageChoice);
    }

    public void SetActiveResultPanel(bool isActive)
    {
        pageResult.SetActive(isActive);
    }

    void ClearAllChoiceButton()
    {
        btnChoice1.gameObject.SetActive(false);
        btnChoice2.gameObject.SetActive(false);
        btnChoice3.gameObject.SetActive(false);
    }

    void ActiveFalseAllPanels()
    {
        pageMain.SetActive(false);
        pageChoice.SetActive(false);
        pageResult.SetActive(false);
        pageReward.SetActive(false);
    }

    void SetPanelsPostion(GameObject first, GameObject second)
    {
        ActiveFalseAllPanels();
        first.SetActive(true);
        second.SetActive(true);

        first.transform.position = firstPanelPos.position;
        second.transform.position = secondPanelPos.position;
    }
}
