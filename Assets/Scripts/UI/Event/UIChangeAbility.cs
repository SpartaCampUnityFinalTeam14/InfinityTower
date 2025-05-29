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
        var ui = UIManager.Instance.GetUI<UIEvent>();
        ui.CloseEvent();
    }

    public void Init(AbilityData beforeData, AbilityData afterData)
    {
        before.Init(beforeData);
        after.Init(afterData);

        before.EnabledButton(false);
        after.EnabledButton(false);
    }

    public void OnButtonClick()
    {
        Hide();

        var ui = UIManager.Instance.GetUI<UIEvent>();
        ui.CloseEvent();
    }
}
