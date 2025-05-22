using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Artifact : MonoBehaviour, ScrollPanel
{
    [SerializeField] private Transform slotParent;
    private List<UI_ArtifactSlot> slots = new();

    [SerializeField] private Button gachaButton;
    [SerializeField] private TextMeshProUGUI gold1Text;
    [SerializeField] private GameObject gachaBackground;
    //[SerializeField] private Button skipBackgroundButton;
    [SerializeField] private Image resultBackground;
    [SerializeField] private Color[] rarityColors;
    [SerializeField] private Image resultImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image boxImage;
    [SerializeField] private Animator boxAnimator;
    [SerializeField] private List<int> boxIndex = new();
    [SerializeField] private Button gachaCloseButton;
    [SerializeField] private Button skipBackgroundButton;

    [SerializeField] private EventChannel OnGoldChanged;
    [SerializeField] private BoolEventChannel OnScrollStateChanged;

    private ArtifactGachaManager gachaManager;
    private bool isShowResultPlaying;
    private Coroutine showResult;

    protected void Awake()
    {
        gachaManager = new();

        gachaButton.onClick.AddListener(GachaArtifact);
        gachaCloseButton.onClick.AddListener(CloseResult);
        skipBackgroundButton.onClick.AddListener(Skip);

        gachaBackground.SetActive(false);

        CheckGachaAble();

        UpdatePriceText();

        Init();
    }

    void Init()
    {
        foreach (Transform child in slotParent) Destroy(child.gameObject);
        slots.Clear();

        foreach(var data in SaveManager.Instance.artifactLevelDict)
        {
            UI_ArtifactSlot slot = Util.InstantiatePrefabAndGetComponent<UI_ArtifactSlot>(path: "UI/Sub/UI_ArtifactSlot", parent: slotParent);
            slots.Add(slot);

            slot.Init(data.Value.id);
        }

        OnGoldChanged.RaiseEvent();
    }

    public void ResetPanel()
    {
        int i = 0;
        foreach (var data in SaveManager.Instance.artifactLevelDict)
        {
            slots[i++].Init(data.Value.id);
        }

        OnGoldChanged.RaiseEvent();
    }

    void Dirty(int id)
    {
        for(int i = 0; i < SaveManager.Instance.artifactLevelDict.Count; i++)
        {
            if (slots[i].id == id)
            {
                slots[i].SetCount(SaveManager.Instance.artifactLevelDict[id].count);
                break;
            }
        }

        OnGoldChanged.RaiseEvent();
    }

    void UpdatePriceText()
    {
        gold1Text.text = $"{gachaManager.GetGachaPrice().ToString():N0}";
    }

    void CheckGachaAble()
    {
        gachaButton.interactable = !gachaManager.IsAllArtifactPulled();
        gachaButton.GetComponentInChildren<TextMeshProUGUI>().text = gachaButton.interactable ? "유물 뽑기" : "전부 뽑음";
    }

    void GachaArtifact()
    {
        if (!SaveManager.Instance.playerData.CheckGold(gachaManager.GetGachaPrice()))
        {
            UIManager.Instance.ShowUI<UI_Alert>().Alert("골드가 부족합니다.");
            return;
        }

        showResult = StartCoroutine(Gacha());
    }

    IEnumerator Gacha()
    {
        OnScrollStateChanged.RaiseEvent(false);

        isShowResultPlaying = true;

        gachaBackground.SetActive(true);
        SetResultActive(false);

        int boxID = boxIndex[Random.Range(0, boxIndex.Count)];
        boxAnimator.SetInteger("BoxID", boxID);
        boxAnimator.Update(0f);

        SaveManager.Instance.playerData.UseGold(gachaManager.GetGachaPrice());
        int id = gachaManager.GetRandomArtifact();
        UpdatePriceText();
        OnGoldChanged.RaiseEvent();
        int rarity = id / 1000;
        resultBackground.color = rarityColors[rarity];

        resultImage.sprite = Resources.Load<Sprite>($"Icons/Artifact/{DataManager.Instance.artifactDicts[id / 1000][id].sprite}");
        nameText.text = DataManager.Instance.artifactDicts[rarity][id].name;

        Dirty(id);

        CheckGachaAble();

        boxAnimator.SetTrigger("Open");
        boxAnimator.Update(0f);
        float clipLength = boxAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        yield return new WaitForSeconds(clipLength);
        SetResultActive(true);

        isShowResultPlaying = false;
    }

    void Skip()
    {
        boxAnimator.SetInteger("BoxID", -1);

        if (isShowResultPlaying)
        {
            StopCoroutine(showResult);
            isShowResultPlaying = false;

            boxAnimator.SetTrigger("Skip");
            boxAnimator.Update(0f);

            SetResultActive(true);
        }
        else
        {
            //boxAnimator.SetTrigger("Close");
            //boxAnimator.Update(0f);

            //CloseResult();
        }
    }

    void SetResultActive(bool flag)
    {
        resultBackground.gameObject.SetActive(flag);
        gachaCloseButton.gameObject.SetActive(flag);
    }

    void CloseResult()
    {
        boxAnimator.SetInteger("BoxID", -1);
        boxAnimator.SetTrigger("Close");
        gachaBackground.SetActive(false);

        OnScrollStateChanged.RaiseEvent(true);
    }
}
