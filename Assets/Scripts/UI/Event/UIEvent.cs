using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIEvent : UI
{
    [Header("sequence Setting")]
    [SerializeField] float OpenDelay;
    [SerializeField] float CloseDelay;

    [Header ("Common Setting")]
    [SerializeField] TextMeshProUGUI eventTitle;
    [SerializeField] Image image;
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject choicePanel;
    [SerializeField] GameObject resultPanel;
    [SerializeField] GameObject rewardPanel;
    [SerializeField] Animator anim;

    [Header("Choice Panel")]
    [SerializeField] TextMeshProUGUI eventDesc;
    [SerializeField] TextMeshProUGUI choiceTitle;
    [SerializeField] Button btnChoice1;
    [SerializeField] Button btnChoice2;
    [SerializeField] Button btnChoice3;

    [Header ("Result Panel")]
    [SerializeField] TextMeshProUGUI resultTitle;
    [SerializeField] TextMeshProUGUI resultDesc;
    [SerializeField] TextMeshProUGUI resultText;
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

    public void ShowEvent(EventData data)
    {
        StartCoroutine(WaitForOpenAnim(data));
    }

    IEnumerator WaitForOpenAnim(EventData data)
    {
        yield return new WaitForSecondsRealtime(OpenDelay);

        anim.SetTrigger("Open");
        anim.Update(0f);
        yield return null;

        while (anim.GetCurrentAnimatorStateInfo(0).IsName("Open") &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        mainPanel.SetActive(true);
        SetEvent(data);
    }

    public void CloseEvent()
    {
        StartCoroutine(WaitForCloseAnim());
    }

    IEnumerator WaitForCloseAnim()
    {
        mainPanel.SetActive(false);


        anim.SetTrigger("Close");
        anim.Update(0f);
        yield return null;

        while (anim.GetCurrentAnimatorStateInfo(0).IsName("Close") &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        yield return new WaitForSecondsRealtime(CloseDelay);

        Hide();
    }

    void SetEvent(EventData data)
    {
        choicePanel.SetActive(true);
        resultPanel.SetActive(false);

        // 메인 이벤트 UI 설정
        eventTitle.text = data.title;
        eventDesc.text = data.description;

        var sprite = Resources.Load<Sprite>($"Event/{data.image}");
        if (sprite) image.sprite = sprite;

        // 이벤트 선택지 설정
        UpdateChoice(data);
    }

    public void SetResult(EventData resultEvent, string reward)
    {
        // 연출
        choicePanel.SetActive(false);
        resultPanel.SetActive(true);

        // Update ResultPanel
        resultTitle.text = resultEvent.title;
        resultDesc.text = resultEvent.description;

        // Update RewardPanel
        resultText.text = resultEvent.choiceTitle;
        btnResult.GetComponentInChildren<TextMeshProUGUI>().text = string.IsNullOrEmpty(resultEvent.result) ? "Event Close" : resultEvent.result;
    }

    public void SetProbabilityEvent(EventData data)
    {
        // 연출
        choicePanel.SetActive(true);
        resultPanel.SetActive(false);

        // Update EventPanel
        eventDesc.text = $"{data.title}\n\n{data.description}";
        
        UpdateChoice(data);
    }

    void UpdateChoice(EventData data)
    {
        choiceTitle.text = data.choiceTitle;

        ClearAllChoiceButton();
        if (!string.IsNullOrEmpty(data.choice1))
        {
            btnChoice1.GetComponentInChildren<TextMeshProUGUI>().text = data.choice1;
            btnChoice1.gameObject.SetActive(true);
        }

        if (!string.IsNullOrEmpty(data.choice2))
        {
            btnChoice2.GetComponentInChildren<TextMeshProUGUI>().text = data.choice2;
            btnChoice2.gameObject.SetActive(true);
        }

        if (!string.IsNullOrEmpty(data.choice3))
        {
            btnChoice3.GetComponentInChildren<TextMeshProUGUI>().text = data.choice3;
            btnChoice3.gameObject.SetActive(true);
        }
    }

    void ClearAllChoiceButton()
    {
        btnChoice1.gameObject.SetActive(false);
        btnChoice2.gameObject.SetActive(false);
        btnChoice3.gameObject.SetActive(false);
    }
}
