using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIChangeAbility : UI
{
    [SerializeField] AbilitySlot before;
    [SerializeField] AbilitySlot after;

    public override void Show()
    {
        base.Show();
    }

    public override void Hide()
    {
        base.Hide();

        // 이벤트창 닫기
        UIManager.Instance.HideUI<UIEvent>();
    }

    public void Init(AbilityData beforeData, AbilityData afterData)
    {
        before.Init(beforeData);
        after.Init(afterData);
    }

    public void OnButtonClick()
    {
        Hide();
    }
}
