using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Shop : UI
{
    [Header("Top")]
    [SerializeField] TextMeshProUGUI textHealth;
    [SerializeField] TextMeshProUGUI textFloor;
    [SerializeField] TextMeshProUGUI textToken;
    [SerializeField] Button btnAbility;

    [Header("Middle")]
    [SerializeField] Transform ParentItems;
    [SerializeField] Button btnRefresh;
    [SerializeField] Button btnUpgrade;
    [SerializeField] Button btnExit;
    [SerializeField] List<ShopItem> itemList;
    [SerializeField] int itemAmount;

    [SerializeField] private EventChannel OnFloorStarted;

    public override void Show()
    {
        base.Show();

        StageManager manager = StageManager.Instance;

        UpdateHealth(manager.GetHP(), manager.GetMaxHP());
        UpdateFloor(manager.GetFloorNum() + 1);
        UpdateToken(manager.token);
        UpdateItemList();
    }

    public void UpdateHealth(float health, float maxHealth)
    {
        textHealth.text = $"{health:N0} / {maxHealth:N0}";
    }

    public void UpdateFloor(int floor)
    {
        textFloor.text = $"{floor}F";
    }

    public void UpdateToken(int token)
    {
        textToken.text = token.ToString("N0");
    }

    public void UpdateItemList()
    {
        List<AbilityData> abilities = GetAbilities();

        for (int i = 0; i < itemList.Count; i++)
            itemList[i].gameObject.SetActive(false);

        // 아이템 세팅
        for (int i = 0; i < abilities.Count; i++)
        {
            itemList[i].InitAbility(this, abilities[i]);
            itemList[i].gameObject.SetActive(true);
        }

        // 마지막 아이템은 항상 전직의 서
        itemList[abilities.Count].InitClassBook(this);
        itemList[abilities.Count].gameObject.SetActive(true);
    }

    public void OnClickAbility()
    {
        UIManager.Instance.GetUI<UIPause>().TogglePause();
    }

    public void OnClickRefresh()
    {
        UpdateItemList();
    }

    public void OnClickUpgrade()
    {
        UIManager.Instance.ShowUI<UI_UpgradeTower>();
    }

    public void OnClickExit()
    {
        Hide();

        UIManager.Instance.GetUI<UI_FloorLoading>().SequenceStart();
        OnFloorStarted.RaiseEvent();
    }

    List<AbilityData> GetAbilities()
    {
        HashSet<int> usedIDs = new HashSet<int>();
        List<AbilityData> result = new List<AbilityData>();

        int maxRetry = 100; // 무한 루프 방지
        int retry = 0;

        while (result.Count < (itemAmount - 1) && retry < maxRetry)
        {
            var ability = StageManager.Instance.abilityManager.GetRandomAbility();

            if (usedIDs.Add(ability.perkID)) // 중복이 아니면 추가
            {
                result.Add(ability);
            }
            else
            {
                retry++;
            }
        }

        return result;
    }
}
