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
    private Coroutine showResult;
    private bool isSkipButtonClicked = false;

    protected override void Awake()
    {
        base.Awake();

        gachaManager = new();
        gachaList.Clear();
        isSkipButtonClicked = false;
        gachaEachBackground.SetActive(false);
        gachaAllBackground.SetActive(false);

        closeButton.onClick.AddListener(() => UIManager.Instance.HideUI<UI_Gacha>());
        gacha1Button.onClick.AddListener(Gacha1);
        gacha10Button.onClick.AddListener(Gacha10);
        gachaEachBackgroundButton.onClick.AddListener(SkipEach);
        gachaAllBackgroundButton.onClick.AddListener(CloseAllResult);

        SetGold(SaveManager.Instance.playerData.gold);
    }

    void Init()
    {

    }

    void SetGold(int gold)
    {
        goldText.text = gold.ToString();
    }

    void Gacha1()
    {
        gachaList.Clear();

        gachaList.Add(gachaManager.GetRandomGacha());

        StartCoroutine(ShowResults());
    }

    void Gacha10()
    {
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

            showResult = StartCoroutine(ShowEachResult(result));
            yield return showResult;
        }

        if(gachaList.Count > 1)
        {
            ShowAllResult();
        }
    }

    IEnumerator ShowEachResult(KeyValuePair<bool, int> result)
    {
        gachaEachBackground.SetActive(true);
        gachaEachResult.Init(result.Key, result.Value);
        gachaEachResult.Hide();

        boxAnimator.SetInteger("BoxID", Random.Range(0, 4));
        yield return new WaitForFixedUpdate();
        boxAnimator.SetTrigger("Open");
        yield return new WaitForFixedUpdate();
        float clipLength = boxAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        yield return new WaitForSeconds(clipLength);

        gachaEachResult.Show();

        yield return new WaitUntil(() => isSkipButtonClicked);
        showResult = null;
    }

    void SkipEach()
    {
        boxAnimator.SetInteger("BoxID", -1);

        if (showResult != null)
        {
            StopCoroutine(showResult);
            showResult = null;

            boxAnimator.SetTrigger("Skip");

            gachaEachResult.Show();
        }
        else
        {
            boxAnimator.SetTrigger("Close");

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
