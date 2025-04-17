using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEvent : UI
{
    [Header ("Event Panel")]
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

    public void SetEventPanel(EventData data)
    {
        eventTitle.text = data.title;
        eventDesc.text = data.description;

        var sprite = Resources.Load<Sprite>($"Event/{data.image}");
        if (sprite) image.sprite = sprite;

        SetChoicePanel(data);

        pageMain1.SetActive(true);
        pageMain2.SetActive(true);
        pageChoice.SetActive(false);
        pageResult.SetActive(false);
    }

    public void SetChoicePanel(EventData data)
    {
        choiceTitle.text = data.choiceTitle;

        ClearAllChoiceButton();
        if (!string.IsNullOrEmpty(data.choice1))
        {
            btnChoice1.GetComponentInChildren<TextMeshProUGUI>().text = data.choice1;
            btnChoice1.enabled = true;
            btnChoice1.gameObject.SetActive(true);
        }

        if (!string.IsNullOrEmpty(data.choice2))
        {
            btnChoice2.GetComponentInChildren<TextMeshProUGUI>().text = data.choice2;
            btnChoice2.enabled = true;
            btnChoice2.gameObject.SetActive(true);
        }

        if (!string.IsNullOrEmpty(data.choice3))
        {
            btnChoice3.GetComponentInChildren<TextMeshProUGUI>().text = data.choice3;
            btnChoice3.enabled = true;
            btnChoice3.gameObject.SetActive(true);
        }
    }

    public void SetResultPanel(EventData resultEvent)
    {
        resultTitle.text = resultEvent.title;
        resultDesc.text = resultEvent.description;
        btnResult.GetComponentInChildren<TextMeshProUGUI>().text = string.IsNullOrEmpty(resultEvent.result) ? "다음 페이지로" : resultEvent.result;
    }

    public void SetRewadText(string str)
    {
        if (!string.IsNullOrEmpty(str))
            resultReward.text = str;
        else
        {
            resultReward.text = "";
        }
    }

    public void SetActiveResultPanel(bool isActive)
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

    public void EnableAllChoiceButton(bool isEnabled)
    {
        btnChoice1.enabled = isEnabled;
        btnChoice2.enabled = isEnabled;
        btnChoice3.enabled = isEnabled;
    }

    void ClearAllChoiceButton()
    {
        btnChoice1.gameObject.SetActive(false);
        btnChoice2.gameObject.SetActive(false);
        btnChoice3.gameObject.SetActive(false);
    }
}
