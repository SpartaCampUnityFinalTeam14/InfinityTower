using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Gacha : MonoBehaviour, ScrollPanel
{
    [SerializeField] private Image boxImage;
    [SerializeField] private Button gacha1Button;
    [SerializeField] private TextMeshProUGUI gold1Text;
    [SerializeField] private Button gacha10Button;
    [SerializeField] private TextMeshProUGUI gold10Text;
    
    [SerializeField] private GameObject gachaEachBackground;
    [SerializeField] private Animator boxAnimator;
    private List<KeyValuePair<bool, int>> gachaList = new();
    [SerializeField] private UI_GachaResult gachaEachResult;
    [SerializeField] private GameObject gachaEachResultBackground;
    [SerializeField] private Button gachaEachBackgroundButton;
    [SerializeField] private Button skipAllButton;

    [SerializeField] private GameObject gachaAllBackground;
    [SerializeField] private Button gachaAllBackgroundButton;
    [SerializeField] private List<UI_GachaResult> gachaAllResult;

    [SerializeField] private EventChannel OnGoldChanged;
    [SerializeField] private BoolEventChannel OnScrollStateChanged;

    private GachaManager gachaManager;
    private int requiredGold;
    private Coroutine showEachResult;
    private Coroutine showResults;
    private bool isShowResultPlaying = false;
    private bool isSkipButtonClicked = false;

    protected void Awake()
    {
        gachaManager = new();

        requiredGold = DataManager.Instance.maginNumberData.gachaRequiredGold;

        gacha1Button.onClick.AddListener(Gacha1);
        gacha10Button.onClick.AddListener(Gacha10);
        gachaEachBackgroundButton.onClick.AddListener(SkipEach);
        skipAllButton.onClick.AddListener(SkipAll);
        gachaAllBackgroundButton.onClick.AddListener(CloseAllResult);

        gold1Text.text = $"{requiredGold.ToString():N0}";
        gold10Text.text = $"{(requiredGold * 10).ToString():N0}";

        ResetPanel();
    }

    public void ResetPanel()
    {
        gachaList.Clear();
        isSkipButtonClicked = false;
        gachaEachBackground.SetActive(false);
        gachaAllBackground.SetActive(false);

        OnGoldChanged.RaiseEvent();
    }

    void Gacha1()
    {
        if (!SaveManager.Instance.playerData.CheckGold(requiredGold))
        {
            UIManager.Instance.ShowUI<UI_Alert>().Alert("골드가 부족합니다.");
            return;
        }
        SaveManager.Instance.playerData.UseGold(requiredGold);
        OnGoldChanged.RaiseEvent();

        skipAllButton.gameObject.SetActive(false);

        gachaList.Clear();

        gachaList.Add(gachaManager.GetRandomGacha());

        showResults = StartCoroutine(ShowResults());
    }

    void Gacha10()
    {
        if (!SaveManager.Instance.playerData.CheckGold(requiredGold * 10))
        {
            UIManager.Instance.ShowUI<UI_Alert>().Alert("골드가 부족합니다.");
            return;
        }
        SaveManager.Instance.playerData.UseGold(requiredGold * 10);
        OnGoldChanged.RaiseEvent();

        skipAllButton.gameObject.SetActive(true);

        gachaList.Clear();

        for (int i = 0; i < 10; i++)
        {
            gachaList.Add(gachaManager.GetRandomGacha());
        }

        showResults = StartCoroutine(ShowResults());
    }

    IEnumerator ShowResults()
    {
        foreach (var result in gachaList)
        {
            OnScrollStateChanged.RaiseEvent(false);

            isSkipButtonClicked = false;
            isShowResultPlaying = false;

            showEachResult = StartCoroutine(ShowEachResult(result));
            yield return new WaitUntil(() => isShowResultPlaying);

            yield return new WaitUntil(() => isSkipButtonClicked);
        }

        if(gachaList.Count > 1)
        {
            OnScrollStateChanged.RaiseEvent(false);

            ShowAllResult();
        }
    }

    IEnumerator ShowEachResult(KeyValuePair<bool, int> result)
    {
        isShowResultPlaying = true;

        gachaEachBackground.SetActive(true);
        gachaEachResult.Init(result.Key, result.Value);
        //gachaEachResult.Hide();
        gachaEachResultBackground.SetActive(false);

        boxAnimator.SetInteger("BoxID", Random.Range(0, 4));
        boxAnimator.Update(0f);
        boxAnimator.SetTrigger("Open");
        boxAnimator.Update(0f);
        float clipLength = boxAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        yield return new WaitForSeconds(clipLength);

        gachaEachResultBackground.SetActive(true);
        //gachaEachResult.Show();

        isShowResultPlaying = false;
    }

    void SkipEach()
    {
        boxAnimator.SetInteger("BoxID", -1);

        if (isShowResultPlaying)
        {
            StopCoroutine(showEachResult);
            isShowResultPlaying = false;

            boxAnimator.SetTrigger("Skip");
            boxAnimator.Update(0f);

            gachaEachResultBackground.SetActive(true);
            //gachaEachResult.Show();
        }
        else
        {
            boxAnimator.SetTrigger("Close");
            boxAnimator.Update(0f);

            isSkipButtonClicked = true;

            gachaEachBackground.SetActive(false);

            OnScrollStateChanged.RaiseEvent(true);
        }
    }

    void SkipAll()
    {
        boxAnimator.SetInteger("BoxID", -1);
        boxAnimator.Update(0f);
        boxAnimator.SetTrigger("Skip");
        boxAnimator.Update(0f);
        boxAnimator.SetTrigger("Close");

        StopCoroutine(showResults);

        if (gachaList.Count > 1)
        {
            ShowAllResult();
        }
    }

    void ShowAllResult()
    {
        gachaEachBackground.SetActive(false);
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

        OnScrollStateChanged.RaiseEvent(true);
    }
}
