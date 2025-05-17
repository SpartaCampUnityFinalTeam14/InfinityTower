using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeTower_Icon : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textName;
    [SerializeField] Image iconImage;
    [SerializeField] Button button;

    public event Action<TowerData> OnClickIcon;

    TowerData data;

    public void Init(TowerData data)
    {
        this.data = data;
        OnClickIcon = null;

        textName.text = data.name;

        var icon = Resources.Load<Sprite>($"Icons/Tower/Tower_{data.id}");
        if (icon) iconImage.sprite = icon;

        //// 타워가 최종 전직일 경우
        //if (data.upgradeTo == -1)
        //{
        //    canvasGroup.alpha = 0.5f;
        //    button.enabled = false;
        //}
        //else
        //{
        //    canvasGroup.alpha = 1;
        //    button.enabled = true;
        //}
    }

    public void OnButtonClick()
    {
        OnClickIcon?.Invoke(data);
    }
}
