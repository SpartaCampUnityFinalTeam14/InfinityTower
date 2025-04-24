using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Gacha : UI
{
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private Image boxImage;
    [SerializeField] private Button gacha1Button;
    [SerializeField] private Button gacha10Button;
    
    [SerializeField] private GameObject gachaEachBackground;
    [SerializeField] private Button gachaEachBackgroundButton;
    [SerializeField] private Animator boxAnimator;
    private List<KeyValuePair<bool, int>> gachaList = new();
    [SerializeField] private UI_GachaResult gachaEachResult;

    [SerializeField] private GameObject gachaAllBackground;
    [SerializeField] private Button gachaAllBackgroundButton;
    [SerializeField] private List<UI_GachaResult> gachaAllResult;

    private GachaManager gachaManager;
    private int requiredGold = 1;
    private Coroutine showResult;
    private bool isShowResultPlaying = false;
    private bool isSkipButtonClicked = false;

    protected override void Awake()
    {
        base.Awake();

        closeButton.onClick.AddListener(() => UIManager.Instance.HideUI<UI_Gacha>());
        gacha1Button.onClick.AddListener(Gacha1);
        gacha10Button.onClick.AddListener(Gacha10);
        gachaEachBackgroundButton.onClick.AddListener(SkipEach);
        gachaAllBackgroundButton.onClick.AddListener(CloseAllResult);

        Init();
    }

    public void Init()
    {
        gachaManager = new();
        gachaList.Clear();
        isSkipButtonClicked = false;
        gachaEachBackground.SetActive(false);
        gachaAllBackground.SetActive(false);

        SetGold(SaveManager.Instance.playerData.gold);
        CheckGacha1Gold();
        CheckGacha10Gold();
    }

    void CheckGacha1Gold()
    {
        gacha1Button.interactable = SaveManager.Instance.playerData.CheckGold(requiredGold);
    }

    void CheckGacha10Gold()
    {
        gacha10Button.interactable = SaveManager.Instance.playerData.CheckGold(requiredGold * 10);
    }

    void SetGold(int gold)
    {
        goldText.text = gold.ToString();
    }

    void Gacha1()
    {
        if (!SaveManager.Instance.playerData.CheckGold(requiredGold)) return;
        SaveManager.Instance.playerData.UseGold(requiredGold);
        CheckGacha1Gold();
        SetGold(SaveManager.Instance.playerData.gold);

        gachaList.Clear();

        gachaList.Add(gachaManager.GetRandomGacha());

        StartCoroutine(ShowResults());
    }

    void Gacha10()
    {
        if (!SaveManager.Instance.playerData.CheckGold(requiredGold * 10)) return;
        SaveManager.Instance.playerData.UseGold(requiredGold * 10);
        CheckGacha10Gold();
        SetGold(SaveManager.Instance.playerData.gold);

        gachaList.Clear();

        for (int i = 0; i < 10; i++)
        {
            gachaList.Add(gachaManager.GetRandomGacha());
        }

        StartCoroutine(ShowResults());
    }

    IEnumerator ShowResults()
    {
        foreach (var result in gachaList)
        {
            isSkipButtonClicked = false;
            isShowResultPlaying = false;

            showResult = StartCoroutine(ShowEachResult(result));
            yield return new WaitUntil(() => isShowResultPlaying);

            yield return new WaitUntil(() => isSkipButtonClicked);
        }

        if(gachaList.Count > 1)
        {
            ShowAllResult();
        }
    }

    IEnumerator ShowEachResult(KeyValuePair<bool, int> result)
    {
        isShowResultPlaying = true;

        Debug.Log($"{result.Key}, {result.Value}");

        gachaEachBackground.SetActive(true);
        gachaEachResult.Init(result.Key, result.Value);
        gachaEachResult.Hide();

        boxAnimator.SetInteger("BoxID", Random.Range(0, 4));
        boxAnimator.Update(0f);
        boxAnimator.SetTrigger("Open");
        boxAnimator.Update(0f);
        float clipLength = boxAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        yield return new WaitForSeconds(clipLength);

        gachaEachResult.Show();

        isShowResultPlaying = false;
    }

    void SkipEach()
    {
        boxAnimator.SetInteger("BoxID", -1);

        if (isShowResultPlaying)
        {
            StopCoroutine(showResult);
            isShowResultPlaying = false;

            boxAnimator.SetTrigger("Skip");
            boxAnimator.Update(0f);

            gachaEachResult.Show();
        }
        else
        {
            boxAnimator.SetTrigger("Close");
            boxAnimator.Update(0f);

            isSkipButtonClicked = true;

            gachaEachBackground.SetActive(false);
        }
    }

    void ShowAllResult()
    {
        gachaAllBackground.SetActive(true);

        for(int i = 0; i < gachaAllResult.Count; i++)
        {
            gachaAllResult[i].Init(gachaList[i].Key, gachaList[i].Value);
        }

        gachaList.Clear();
    }

    void CloseAllResult()
    {
        gachaAllBackground.SetActive(false);
    }

    public override void Clear()
    {
        base.Clear();
    }
}
