using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    enum Shop_ItemType { Ability, ClassBook, Potion }

    [SerializeField] TextMeshProUGUI textName;
    [SerializeField] TextMeshProUGUI textPrice;
    [SerializeField] TextMeshProUGUI textAmount;
    [SerializeField] Button btnBuy;
    [SerializeField] Image itemImage;
    [SerializeField] Image SoldOut;

    UI_Shop uiShop;
    AbilityData data;
    int amount;
    int price;
    Shop_ItemType type;

    public void InitAbility(UI_Shop ui, AbilityData data)
    {
        uiShop = ui;
        this.data = data;
        type = Shop_ItemType.Ability;
        btnBuy.enabled = true;
        amount = 1;

        textName.text = data.name;

        string test = $"Icons/Ability/{data.image}";

        var icon = Resources.Load<Sprite>($"Icons/Ability/{data.image}");
        if (icon) itemImage.sprite = icon;

        switch(data.rarity)
        {
            case (int)Rarity.Common:
                price = Random.Range(18, 23) * 10;
                break;
            case (int)Rarity.Rare:
                price = Random.Range(25, 36) * 10;
                break;
            case (int)Rarity.Epic:
                price = Random.Range(47, 56) * 10;
                break;
        }

        textPrice.text = price.ToString();

        textAmount.gameObject.SetActive(false);
        SoldOut.gameObject.SetActive(false);
    }

    public void InitClassBook(UI_Shop ui)
    {
        uiShop = ui;
        type = Shop_ItemType.ClassBook;
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

        SoldOut.gameObject.SetActive(false);

    }

    public void InitPotion(UI_Shop ui)
    {
        uiShop = ui;
        type = Shop_ItemType.Potion;
        btnBuy.enabled = true;

        textName.text = "회복 물약";

        price = 200;
        textPrice.text = price.ToString();

        textAmount.gameObject.SetActive(false);
        SoldOut.gameObject.SetActive(false);
    }

    public void OnItemClick()
    {
        Debug.Log("아이템 구매");

        if (StageManager.Instance.CheckToken(price))
        {
            StageManager.Instance.UseToken(price);
            uiShop.UpdateToken(StageManager.Instance.token);

            if (type == Shop_ItemType.Ability) StageManager.Instance.abilityManager.AddAbillity(data);
            else if (type == Shop_ItemType.ClassBook) StageManager.Instance.GainBook(amount);
            else if (type == Shop_ItemType.Potion) StageManager.Instance.Heal(20);

            uiShop.UpdateHealth(StageManager.Instance.GetHP(), StageManager.Instance.GetMaxHP());
            textPrice.text = "Sold Out";
            SoldOut.gameObject.SetActive(true);
            btnBuy.enabled = false;
        }
        else
        {
            UIManager.Instance.ShowUI<UI_Alert>().Alert("주화가 부족합니다.");
        }
    }
}
