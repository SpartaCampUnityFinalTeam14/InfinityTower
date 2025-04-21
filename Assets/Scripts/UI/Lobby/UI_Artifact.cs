using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Artifact : UI
{
    [SerializeField] private Button closeButton;

    [SerializeField] private Transform slotParent;
    private List<UI_ArtifactSlot> slots = new();

    [SerializeField] private Button gachaButton;

    [SerializeField] private GameObject gachaBackground;
    [SerializeField] private Image resultBackground;
    [SerializeField] private Color[] rarityColors;
    [SerializeField] private Image resultImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image boxImage;
    [SerializeField] private Button gachaCloseButton;

    private ArtifactGachaManager gachaManager;

    protected override void Awake()
    {
        base.Awake();

        gachaManager = new();

        closeButton.onClick.AddListener(() => UIManager.Instance.HideUI<UI_Artifact>());
        gachaButton.onClick.AddListener(Gacha);
        gachaCloseButton.onClick.AddListener(() => gachaBackground.SetActive(false));

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
    }

    void CheckGachaAble()
    {
        gachaButton.interactable = !gachaManager.IsAllArtifactPulled();
        gachaButton.GetComponentInChildren<TextMeshProUGUI>().text = gachaButton.interactable ? "유물 뽑기" : "전부 뽑음";
    }

    void Gacha()
    {
        gachaBackground.SetActive(true);

        int id = gachaManager.GetRandomArtifact();
        int rarity = id / 1000;
        resultBackground.color = rarityColors[rarity];

        //resultImage 세팅
        nameText.text = DataManager.Instance.artifactDicts[rarity][id].name;

        Dirty(id);

        CheckGachaAble();
    }

    public override void Clear()
    {

    }
}
