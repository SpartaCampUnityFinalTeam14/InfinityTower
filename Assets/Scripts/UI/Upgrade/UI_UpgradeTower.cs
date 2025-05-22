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
    [SerializeField] TextMeshProUGUI infoName_befor;
    [SerializeField] Image infoIcon_befor;
    [SerializeField] TextMeshProUGUI infoName_after;
    [SerializeField] Image infoIcon_after;
    [SerializeField] TextMeshProUGUI upgradeCost;

    [Header("UpgradeResult")]
    [SerializeField] GameObject upgradeResult;
    [SerializeField] TextMeshProUGUI resultName_befor;
    [SerializeField] Image resultIcon_befor;
    [SerializeField] TextMeshProUGUI resultName_after;
    [SerializeField] Image resultIcon_after;

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
            towerList[idx].OnClickIcon += ShowUpgradeInfo;
            idx++;
        }

        textBook.text = StageManager.Instance.book.ToString();

        upgradeInfo.SetActive(false);
    }

    void ShowUpgradeInfo(TowerData data)
    {
        if (data.upgradeTo == -1) // 타워가 이미 최종 단계일 경우
        {
            UIManager.Instance.ShowUI<UI_Alert>().Alert("이미 최종 단계입니다.");
            upgradeInfo.SetActive(false);
            selectedTower = null;

            return;
        }

        upgradeInfo.SetActive(true);

        TowerData upgradeTower = DataManager.Instance.towerDict[data.upgradeTo];

        infoName_befor.text = data.name;
        infoName_after.text = upgradeTower.name;

        var icon = Resources.Load<Sprite>($"Icons/Tower/Tower_{data.id}");
        if (icon) infoIcon_befor.sprite = icon;

        icon = Resources.Load<Sprite>($"Icons/Tower/Tower_{upgradeTower.id}");
        if (icon) infoIcon_after.sprite = icon;

        upgradeCost.text = $"-{(data.id % 3) + 2}";

        selectedTower = data;
    }

    void ShowUpgradeResult(TowerData data)
    {
        upgradeResult.SetActive(true);

        TowerData upgradeTower = DataManager.Instance.towerDict[data.upgradeTo];

        resultName_befor.text = data.name;
        resultName_after.text = upgradeTower.name;

        var icon = Resources.Load<Sprite>($"Icons/Tower/Tower_{data.id}");
        if (icon) resultIcon_befor.sprite = icon;

        icon = Resources.Load<Sprite>($"Icons/Tower/Tower_{upgradeTower.id}");
        if (icon) resultIcon_after.sprite = icon;
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

                ShowUpgradeResult(selectedTower);
                Init();
            }
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
