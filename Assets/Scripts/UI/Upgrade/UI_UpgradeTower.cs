using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_UpgradeTower : UI
{
    [SerializeField] TextMeshProUGUI textBook;
    [SerializeField] List<UpgradeTower_Icon> towerList;

    [Header("UpgradeInfo")]
    [SerializeField] GameObject upgradeInfo;
    [SerializeField] TextMeshProUGUI infoName_Origin;
    [SerializeField] TextMeshProUGUI infoName_Upgrade;
    [SerializeField] Image infoIcon_Origin;
    [SerializeField] Image infoIcon_Upgrade;

    [Header("UpgradeResult")]
    [SerializeField] GameObject upgradeResult;
    [SerializeField] TextMeshProUGUI resultName_Origin;
    [SerializeField] TextMeshProUGUI resultName_Upgrade;
    [SerializeField] Image resultIcon_Origin;
    [SerializeField] Image resultIcon_Upgrade;
    [SerializeField] TextMeshProUGUI Title_OriginStat;
    [SerializeField] TextMeshProUGUI Title_UpgradeStat;
    [SerializeField] TextMeshProUGUI Value_OriginStat;
    [SerializeField] TextMeshProUGUI Value_UpgradeStat;
    [SerializeField] TextMeshProUGUI upgradeCost;

    TowerData selectedTower;

    public override void Show()
    {
        base.Show();

        Init();
    }

    void Init()
    {
        int idx = 0;
        foreach (var towerID in StageManager.Instance.selectedTowers)
        {
            towerList[idx].Init(DataManager.Instance.towerDict[towerID]);
            towerList[idx].OnClickIcon += ShowUpgradeResult;
            idx++;
        }

        textBook.text = StageManager.Instance.book.ToString();

        //upgradeInfo.SetActive(false);
        upgradeResult.SetActive(false);
    }

    void ShowUpgradeInfo(TowerData originData)
    {
        if (originData.upgradeTo == -1) // 타워가 이미 최종 단계일 경우
        {
            UIManager.Instance.ShowUI<UI_Alert>().Alert("이미 최종 단계입니다.");
            upgradeInfo.SetActive(false);
            selectedTower = null;

            return;
        }

        upgradeInfo.SetActive(true);

        TowerData upgradeTower = DataManager.Instance.towerDict[originData.upgradeTo];

        // 타워 이름 업데이트
        infoName_Origin.text = originData.name;
        infoName_Upgrade.text = upgradeTower.name;

        // 타워 아이콘 업데이트
        var icon = Resources.Load<Sprite>($"Icons/Tower/Tower_{originData.id}");
        if (icon) infoIcon_Origin.sprite = icon;

        icon = Resources.Load<Sprite>($"Icons/Tower/Tower_{upgradeTower.id}");
        if (icon) infoIcon_Upgrade.sprite = icon;

        upgradeCost.text = $"-{(originData.id % 3) + 2}";

        selectedTower = originData;
    }

    void ShowUpgradeResult(TowerData originData)
    {
        // 타워가 이미 최종 단계일 경우
        if (originData.upgradeTo == -1) 
        {
            UIManager.Instance.ShowUI<UI_Alert>().Alert("이미 최종 단계입니다.");
            upgradeInfo.SetActive(false);
            selectedTower = null;

            return;
        }

        selectedTower = originData;
        TowerData upgradeData = DataManager.Instance.towerDict[originData.upgradeTo];

        // 타워 이름 업데이트
        resultName_Origin.text = originData.name;
        resultName_Upgrade.text = upgradeData.name;

        // 타워 아이콘 업데이트
        var icon = Resources.Load<Sprite>($"Icons/Tower/Tower_{originData.id}");
        if (icon) resultIcon_Origin.sprite = icon;

        icon = Resources.Load<Sprite>($"Icons/Tower/Tower_{upgradeData.id}");
        if (icon) resultIcon_Upgrade.sprite = icon;

        // Tower Stat Update
        Title_OriginStat.text = string.Empty;
        Title_UpgradeStat.text = string.Empty;
        Value_OriginStat.text = string.Empty;
        Value_UpgradeStat.text = string.Empty;

        for (int i = 0; i < originData.statType.Count; i++)
        {
            Title_OriginStat.text += DataManager.Instance.statusDict[originData.statType[i]].name + '\n';
            Value_OriginStat.text += originData.statValue[i].ToString("N1") + '\n';
        }

        for (int i = 0; i < upgradeData.statType.Count; i++)
        {
            Title_UpgradeStat.text += DataManager.Instance.statusDict[upgradeData.statType[i]].name + '\n';
            Value_UpgradeStat.text += upgradeData.statValue[i].ToString("N1") + '\n';
        }

        // Cost Text Update
        upgradeCost.text = $"-{(originData.id % 3) + 2}";

        upgradeResult.SetActive(true);
    }

    public void OnClickUpgrade()
    {
        if (selectedTower == null) return;

        int cost = (selectedTower.id % 3) + 2;
        if (StageManager.Instance.CheckBook(cost))
        {
            StageManager.Instance.UseBook(cost);

            int idx = StageManager.Instance.selectedTowers.IndexOf(selectedTower.id);

            SendAnalytics();

            if (idx != -1)
            {
                StageManager.Instance.selectedTowers[idx] = DataManager.Instance.towerDict[selectedTower.id].upgradeTo;
                //ShowUpgradeResult(selectedTower);
            }

            OnClickResultExit();
        }
        else
        {
            UIManager.Instance.ShowUI<UI_Alert>().Alert("전직의 서가 부족합니다.");
        }
    }

    public void OnClickExitButton()
    {
        Hide();
    }

    public void OnClickResultExit()
    {
        Init();

        upgradeResult.SetActive(false);
    }

    private void SendAnalytics()
    {
        Debug.Log(StageManager.Instance.GetFloorNum());
        
        AnalyticsManager.SendEvent("TOWER_UPGRADE", new Dictionary<string, object>
        {
            { "TOWER_TYPE", DataManager.Instance.towerDict[selectedTower.id].name },
            { "STAGE_NUMBER", StageManager.Instance.GetStageNum() },
            { "FLOOR_NUMBER", StageManager.Instance.GetFloorNum() }
        });
    }
}
