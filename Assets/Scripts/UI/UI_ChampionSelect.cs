using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChampionSelect : UI
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Image championImage;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Image Skill1Image;
    [SerializeField] private Image Skill2Image;

    [SerializeField] private Transform slotParent;
    private List<UI_ChampionSlot> slots = new();

    protected override void Awake()
    {
        base.Awake();

        closeButton.onClick.AddListener(() => UIManager.Instance.HideUI<UI_ChampionSelect>());

        Init();
    }

    void Init()
    {
        foreach(Transform child in slotParent) Destroy(child.gameObject);
        slots.Clear();

        for(int i = 0; i < DataManager.Instance.championDict.Count; i++)
        {
            UI_ChampionSlot slot = Util.InstantiatePrefabAndGetComponent<UI_ChampionSlot>(path: "UI/Sub/UI_ChampionSlot", parent: slotParent);
            slots.Add(slot);

            slot.Init(DataManager.Instance.championDict[i].id);
        }
    }

    public override void Clear()
    {

    }
}
