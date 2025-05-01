using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Artifact : UI
{
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI goldText;

    [SerializeField] private Transform slotParent;
    private List<UI_ArtifactSlot> slots = new();

    [SerializeField] private Button gachaButton;

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

    private ArtifactGachaManager gachaManager;

    protected override void Awake()
    {
        base.Awake();

        gachaManager = new();

        closeButton.onClick.AddListener(() => UIManager.Instance.HideUI<UI_Artifact>());
        gachaButton.onClick.AddListener(() => StartCoroutine(Gacha()));
        gachaCloseButton.onClick.AddListener(() => 
        {
            boxAnimator.SetInteger("BoxID", -1);
            boxAnimator.SetTrigger("Close");
            gachaBackground.SetActive(false);
        });

        gachaBackground.SetActive(false);

        CheckGachaAble();

        Init();
    }

    void Init()
    {
        foreach (Transform child in slotParent) Destroy(child.gameObject);
        slots.Clear();

        foreach(var data in SaveManager.Instance.artifactSaveDict)
        {
            UI_ArtifactSlot slot = Util.InstantiatePrefabAndGetComponent<UI_ArtifactSlot>(path: "UI/Sub/UI_ArtifactSlot", parent: slotParent);
            slots.Add(slot);

            slot.Init(data.Value.id);
        }

        UpdateGold();
    }

    void Dirty(int id)
    {
        for(int i = 0; i < SaveManager.Instance.artifactSaveDict.Count; i++)
        {
            if (slots[i].id == id)
            {
                slots[i].SetCount(SaveManager.Instance.artifactSaveDict[id].count);
                return;
            }
        }

        UpdateGold();
    }

    void UpdateGold()
    {
        int gold = SaveManager.Instance.playerData.gold;
        goldText.text = string.Format("{0:N0}", gold);
    }

    void CheckGachaAble()
    {
        gachaButton.interactable = !gachaManager.IsAllArtifactPulled();
        gachaButton.GetComponentInChildren<TextMeshProUGUI>().text = gachaButton.interactable ? "유물 뽑기" : "전부 뽑음";
    }

    IEnumerator Gacha()
    {
        gachaBackground.SetActive(true);
        SetResultActive(false);

        int boxID = boxIndex[Random.Range(0, boxIndex.Count)];
        Debug.Log(boxAnimator.GetInteger("BoxID"));
        boxAnimator.SetInteger("BoxID", boxID);
        yield return new WaitForFixedUpdate();
        Debug.Log(boxAnimator.GetInteger("BoxID"));

        int id = gachaManager.GetRandomArtifact();
        int rarity = id / 1000;
        resultBackground.color = rarityColors[rarity];

        //resultImage 세팅
        nameText.text = DataManager.Instance.artifactDicts[rarity][id].name;

        Dirty(id);

        CheckGachaAble();

        boxAnimator.SetTrigger("Open");
        yield return new WaitForFixedUpdate();
        float clipLength = boxAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        yield return new WaitForSeconds(clipLength);
        SetResultActive(true);
    }

    void SetResultActive(bool flag)
    {
        resultBackground.gameObject.SetActive(flag);
        gachaCloseButton.gameObject.SetActive(flag);
    }

    public override void Clear()
    {

    }
}
