using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEvent : UI
{
    [Header ("Event Page")]
    [SerializeField] GameObject pageMain1;
    [SerializeField] GameObject pageMain2;
    [SerializeField] GameObject pageChoice;
    [SerializeField] GameObject pageResult;

    [Header("MainEvent")]
    [SerializeField] TextMeshProUGUI eventTitle;
    [SerializeField] TextMeshProUGUI eventDesc;
    [SerializeField] Button btnNext;
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

    EventData eventData;

    protected override void Awake()
    {
        base.Awake();

        btnNext.onClick.AddListener(OnClickNextButton);
        btnChoice1.onClick.AddListener(() => StageManager.Instance.eventManager.SelectChoice(0));
        btnChoice2.onClick.AddListener(() => StageManager.Instance.eventManager.SelectChoice(1));
        btnChoice3.onClick.AddListener(() => StageManager.Instance.eventManager.SelectChoice(2));
        btnResult.onClick.AddListener(StageManager.Instance.eventManager.OnClickResultButton);
    }

    public override void Show()
    {
        base.Show();

        Time.timeScale = 0f;
        StageManager.Instance.isEventEnd = false;
    }

    public override void Hide()
    {
        base.Hide();

        if (!StageManager.Instance.isPause && StageManager.Instance.CurFloor.isPerkSelected)
            Time.timeScale = 1f;

        StageManager.Instance.isEventEnd = true;
    }

    public void SetEventText(EventData data)
    {
        eventData = data;

        eventTitle.text = eventData.title;
        eventDesc.text = eventData.description;
        choiceTitle.text = eventData.choiceTitle;

        var sprite = Resources.Load<Sprite>($"Event/{eventData.image}");
        if (sprite) image.sprite = sprite;

        ClearAllChoiceButton();
        if (!string.IsNullOrEmpty(eventData.choice1))
        {
            btnChoice1.GetComponentInChildren<TextMeshProUGUI>().text = eventData.choice1;
            btnChoice1.gameObject.SetActive(true);
            btnChoice1.enabled = true;
        }

        if (!string.IsNullOrEmpty(eventData.choice2))
        {
            btnChoice2.GetComponentInChildren<TextMeshProUGUI>().text = eventData.choice2;
            btnChoice2.gameObject.SetActive(true);
            btnChoice2.enabled = true;
        }

        if (!string.IsNullOrEmpty(eventData.choice3))
        {
            btnChoice3.GetComponentInChildren<TextMeshProUGUI>().text = eventData.choice3;
            btnChoice3.gameObject.SetActive(true);
            btnChoice3.enabled = true;
        }

        pageMain1.SetActive(true);
        pageMain2.SetActive(true);
        pageChoice.SetActive(false);
        pageResult.SetActive(false);
    }

    public void SetResultText(EventData resultEvent)
    {
        resultTitle.text = resultEvent.choiceTitle;
        resultDesc.text = resultEvent.description;
        btnResult.GetComponentInChildren<TextMeshProUGUI>().text = resultEvent.buttonText;
    }

    public void SetRewadText(string str)
    {
        if (!string.IsNullOrEmpty(str))
            resultReward.text = $"{str} 획득";
        else
        {
            resultReward.text = "";
        }
    }

    public void SetActiveResult(bool isActive)
    {
        pageResult.SetActive(isActive);
    }

    public void OnClickNextButton()
    {
        pageMain1.SetActive(false);
        pageMain2.SetActive(false);
        pageChoice.SetActive(true);
        pageResult.SetActive(false);
    }

    public void DisableAllChoiceButton()
    {
        btnChoice1.enabled = false;
        btnChoice2.enabled = false;
        btnChoice3.enabled = false;
    }

    void ClearAllChoiceButton()
    {
        btnChoice1.gameObject.SetActive(false);
        btnChoice2.gameObject.SetActive(false);
        btnChoice3.gameObject.SetActive(false);
    }
}
