using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFortuneEvent : UI
{
    [SerializeField] Transform iconParent;
    [SerializeField] AbilitySlot abilitSlot;
    [SerializeField] Button btnChange;

    List<AbilityIcon> listIcon;
    AbilityData selectedData;

    protected override void Awake()
    {
        base.Awake();

        listIcon = new List<AbilityIcon>();
    }

    public override void Show()
    {
        base.Show();

        InitAbilityIcon();
        abilitSlot.gameObject.SetActive(false);
        btnChange.enabled = false;
    }

    public override void Hide()
    {
        base.Hide();

        ReleaseIcons();
        abilitSlot.gameObject.SetActive(false);
    }

    void InitAbilityIcon()
    {
        var abilities = StageManager.Instance.abilityManager.abilities;
        GameObject iconPrefab = Resources.Load<GameObject>("Prefabs/Ability/AbilityIcon");

        foreach (var ability in abilities.Values) 
        {
            var icon = PoolManager.Instance.Get(iconPrefab, 20, iconParent).GetComponent<AbilityIcon>();
            icon.Init(ability.Data, true);
            icon.clickEvent += SetAbilitySlot;

            listIcon.Add(icon);
        }
    }

    void ReleaseIcons()
    {
        foreach (var icon in listIcon)
        {
            PoolManager.Instance.Release(icon);
        }

        listIcon.Clear();
    }

    void SetAbilitySlot(AbilityData data)
    {
        selectedData = data;

        abilitSlot.Init(data);
        abilitSlot.gameObject.SetActive(true);
        btnChange.enabled = true;
    }

    public void OnbuttonChange()
    {
        // 선택된 특성과 같은 등급의 특성 랜덤 뽑기
        var newAbility = StageManager.Instance.abilityManager.GetRandomAbility(selectedData.rarity);

        StageManager.Instance.abilityManager.RemoveAbility(selectedData);
        StageManager.Instance.abilityManager.AddAbillity(newAbility);

        var uiResult = UIManager.Instance.ShowUI<UIChangeAbility>();
        uiResult.Init(selectedData, newAbility);

        Hide();
    }
}
