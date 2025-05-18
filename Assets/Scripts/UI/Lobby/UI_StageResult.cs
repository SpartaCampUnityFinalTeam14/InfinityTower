using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_StageResult : UI
{
    [Header("좌측")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Image championIcon;
    [SerializeField] private TextMeshProUGUI championNameText;
    [SerializeField] private List<Image> towerIcons;
    [SerializeField] private List<TextMeshProUGUI> towerNameTexts;
    [Header("우측")]
    [SerializeField] private TextMeshProUGUI timeSumText;
    [SerializeField] private TextMeshProUGUI floorCountText;
    [SerializeField] private TextMeshProUGUI rewardGoldText;
    [SerializeField] private Transform content;
    [SerializeField] private Button closeButton;

    protected override void Awake()
    {
        base.Awake();

        closeButton.onClick.AddListener(ToLobby);

        int championId = SaveManager.Instance.playerData.selectedChampionIndex;
        championIcon.sprite = Resources.Load<Sprite>($"Icons/Champion/Champion_{championId}");
        championNameText.text = DataManager.Instance.championDict[championId].name;

        for(int i = 0; i < SaveManager.Instance.playerData.selectedTowerIndex.Count; i++)
        {
            int towerId = SaveManager.Instance.playerData.selectedTowerIndex[i];
            towerIcons[i].sprite = Resources.Load<Sprite>($"Icons/Tower/Tower_{towerId}");
            towerNameTexts[i].text = DataManager.Instance.towerDict[towerId].name;
        }
    }

    public void Init(bool isSuccess, int time, int floorCount, int rewardGold)
    {
        titleText.text = isSuccess ? "탐험 성공" : "탐험 실패";
        timeSumText.text = Util.FormatTimeMMSS(time);
        floorCountText.text = floorCount.ToString() + "층";
        rewardGoldText.text = rewardGold.ToString() + "G";

        InitAbility();
    }

    void InitAbility()
    {
        foreach(Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach(Ability ability in StageManager.Instance.abilityManager.allAbilities.Values)
        {
            UI_ResultAbilitySlot slot = Util.InstantiatePrefabAndGetComponent<UI_ResultAbilitySlot>(path: "UI/Sub/UI_ResultAbilitySlot", parent: content);
            slot.Init(ability.Data.perkID, ability.CurStackCount);
        }
    }

    void ToLobby()
    {
        StageManager.Instance.timeScaleManager.PopTimeScale();
        GameManager.Instance.LoadScene("KSM_Lobby");
    }

    public override void Clear()
    {
        base.Clear();
    }
}
