using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_ItemInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemDesc;
    [SerializeField] Image itemImage;

    public event Action OnBuyItem;

    public void Init(AbilityData data, Sprite icon)
    {
        OnBuyItem = null;

        itemName.text = data.name;
        itemDesc.text = data.description;
        itemImage.sprite = icon;

    }

    public void OnClickBuyItem()
    {
        Debug.Log("아이템 구매");

        OnBuyItem?.Invoke();
        gameObject.SetActive(false);
    }

    public void OnClickClose()
    {
        gameObject.SetActive(false);
    }
}
