using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEvent : UI, IPointerClickHandler
{
    [Header("sequence Setting")]
    [SerializeField] float openDelay;
    [SerializeField] float closeDelay;
    [SerializeField] float printTextDelay;
    
    [Header ("Common Setting")]
    [SerializeField] TextMeshProUGUI eventTitle;
    [SerializeField] Image image;
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject choicePanel;
    [SerializeField] GameObject resultPanel;
    [SerializeField] Animator anim;

    [Header("Choice Panel")]
    [SerializeField] TextMeshProUGUI eventDesc;
    [SerializeField] TextMeshProUGUI choiceTitle;
    [SerializeField] GameObject layoutChoice;
    [SerializeField] Button btnChoice1;
    [SerializeField] Button btnChoice2;
    [SerializeField] Button btnChoice3;

    [Header ("Result Panel")]
    [SerializeField] TextMeshProUGUI resultTitle;
    [SerializeField] TextMeshProUGUI resultDesc;
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] GameObject layoutResult;
    [SerializeField] Button btnResult;

    [Header("Reward Panel")]
    [SerializeField] GameObject rewardPanel;
    [SerializeField] List<AbilitySlot> abilitySlots;

    Coroutine coroutine;
    WaitForSecondsRealtime wait;
    bool isSkip;

    protected override void Awake()
    {
        base.Awake();

        btnChoice1.onClick.AddListener(() => StageManager.Instance.eventManager.SelectChoice(0));
        btnChoice2.onClick.AddListener(() => StageManager.Instance.eventManager.SelectChoice(1));
        btnChoice3.onClick.AddListener(() => StageManager.Instance.eventManager.SelectChoice(2));
        btnResult.onClick.AddListener(StageManager.Instance.eventManager.OnClickResultButton);

        wait = new WaitForSecondsRealtime(printTextDelay);
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
        mainPanel.SetActive(false);

        StartCoroutine(WaitForOpenAnim(data));
    }

    public void CloseEvent()
    {
        StartCoroutine(WaitForCloseAnim());
    }

    IEnumerator WaitForOpenAnim(EventData data)
    {
        yield return new WaitForSecondsRealtime(openDelay);
        
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

        yield return new WaitForSecondsRealtime(closeDelay);

        Hide();
    }

    void SetEvent(EventData data)
    {
        choicePanel.SetActive(true);
        resultPanel.SetActive(false);
        layoutChoice.SetActive(false);

        StartCoroutine(SetEventCoroutine(data));
    }

    IEnumerator SetEventCoroutine(EventData data)
    {
        eventTitle.text = data.title;

        var sprite = Resources.Load<Sprite>($"Icons/Event/{data.image}");
        if (sprite) image.sprite = sprite;

        yield return coroutine = StartCoroutine(PrintText(eventDesc, data.description));

        // 이벤트 선택지 설정
        UpdateChoice(data);
    }

    public void SetResult(EventData resultEvent, string reward)
    {
        choicePanel.SetActive(false);
        resultPanel.SetActive(true);
        layoutResult.SetActive(false);

        StartCoroutine(SetResultCoroutine(resultEvent));
    }

    IEnumerator SetResultCoroutine(EventData data)
    {
        resultTitle.text = data.title;

        var sprite = Resources.Load<Sprite>($"Icons/Event/{data.image}");
        if (sprite) image.sprite = sprite;

        yield return coroutine = StartCoroutine(PrintText(resultDesc, data.description));

        // Update RewardPanel
        layoutResult.SetActive(true);
        resultText.text = data.choiceTitle;
        btnResult.GetComponentInChildren<TextMeshProUGUI>().text = string.IsNullOrEmpty(data.result) ? "Event Close" : data.result;
    }

    public void SetProbabilityEvent(EventData data)
    {
        choicePanel.SetActive(true);
        resultPanel.SetActive(false);
        layoutChoice.SetActive(false);

        StartCoroutine(SetProbabilityCoroutine(data));
    }

    IEnumerator SetProbabilityCoroutine(EventData data)
    {
        var sprite = Resources.Load<Sprite>($"Icons/Event/{data.image}");
        if (sprite) image.sprite = sprite;

        yield return coroutine = StartCoroutine(PrintText(eventDesc, $"{data.title}\n\n{data.description}"));

        // 이벤트 선택지 설정
        UpdateChoice(data);
    }

    public void SetReward(List<AbilityData> list)
    {
        if (list.Count <= 0)
            return;

        for (int i = 0; i < list.Count; i++)
        {
            abilitySlots[i].Init(list[i]);

            abilitySlots[i].gameObject.SetActive(true);
        }

        rewardPanel.SetActive(true);
    }

    void UpdateChoice(EventData data)
    {
        layoutChoice.SetActive(true);
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

    IEnumerator PrintText(TextMeshProUGUI textMesh, string text)
    {
        int cnt = 0;
        textMesh.text = string.Empty;

        while (cnt < text.Length) 
        {
            if (isSkip)
                break;

            textMesh.text += text[cnt];
            cnt++;

            yield return wait;
        }

        textMesh.text = text;

        isSkip = false;
        coroutine = null;

        yield return null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (coroutine == null)
            return;

        isSkip = true;
    }

    public void OnRewardClose()
    {
        for (int i = 0; i < abilitySlots.Count; i++)
        {
            abilitySlots[i].gameObject.SetActive(false);
        }

        rewardPanel.SetActive(false);

        var ui = UIManager.Instance.GetUI<UIEvent>();
        ui.CloseEvent();
    }
}
