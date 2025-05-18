using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textName;
    [SerializeField] TextMeshProUGUI textPrice;
    [SerializeField] TextMeshProUGUI textAmount;
    [SerializeField] Button btnBuy;
    [SerializeField] Image itemImage;

    UI_Shop uiShop;
    AbilityData data;
    int amount;
    int price;
    bool isAbility;

    public void InitAbility(UI_Shop ui, AbilityData data)
    {
        uiShop = ui;
        this.data = data;
        isAbility = true;
        btnBuy.enabled = true;
        amount = 1;

        textName.text = data.name;

        //string test = $"Icons/Ability/{data.image}";

        var icon = Resources.Load<Sprite>($"Icons/Ability/{data.image}");
        if (icon) itemImage.sprite = icon;

        switch(data.rarity)
        {
            case (int)Rarity.Common:
                price = Random.Range(180, 220);
                break;
            case (int)Rarity.Rare:
                price = Random.Range(250, 350);
                break;
            case (int)Rarity.Epic:
                price = Random.Range(475, 550);
                break;
        }

        textPrice.text = price.ToString();
        textAmount.gameObject.SetActive(false);
    }

    public void InitClassBook(UI_Shop ui)
    {
        uiShop = ui;
        isAbility = false;
        btnBuy.enabled = true;
        amount = Random.Range(1, 4);

        textName.text = "전직의 서";

        switch (amount)
        {
            case 1:
                price = 200;
                break;
            case 2:
                price = 350;
                break;
            case 3:
                price = 475;
                break;
        }

        textPrice.text = price.ToString();

        textAmount.gameObject.SetActive(true);
        textAmount.text = $"x{amount}";
    }

    public void OnItemClick()
    {
        Debug.Log("아이템 구매");

        if (StageManager.Instance.CheckToken(price))
        {
            StageManager.Instance.UseToken(price);
            uiShop.UpdateToken(StageManager.Instance.token);

            if (isAbility) StageManager.Instance.abilityManager.AddAbillity(data);
            else StageManager.Instance.GainBook(amount);

            textPrice.text = "Sold Out";
            btnBuy.enabled = false;
        }
        else
        {
            UIManager.Instance.ShowUI<UI_Alert>().Alert("주화가 부족합니다.");
        }
    }
}
