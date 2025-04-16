using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Gacha : UI
{
    [SerializeField] private Button closeButton;

    [SerializeField] private Transform slotParent;
    private List<UI_ArtifactSlot> slots = new();

    [SerializeField] private Button gachaButton;
    [SerializeField] private Image resultImage;
    [SerializeField] private TextMeshProUGUI nameText;

    protected override void Awake()
    {
        base.Awake();

        closeButton.onClick.AddListener(() => UIManager.Instance.HideUI<UI_Gacha>());

        Init();
    }

    void Init()
    {
        foreach (Transform child in slotParent) Destroy(child.gameObject);
        slots.Clear();

        for (int i = 0; i < DataManager.Instance.championDict.Count; i++)
        {
            UI_ArtifactSlot slot = Util.InstantiatePrefabAndGetComponent<UI_ArtifactSlot>(path: "UI/Sub/UI_ArtifactSlot", parent: slotParent);
            slots.Add(slot);

            slot.Init(DataManager.Instance.championDict[i].id);
        }
    }

    public override void Clear()
    {

    }
}
