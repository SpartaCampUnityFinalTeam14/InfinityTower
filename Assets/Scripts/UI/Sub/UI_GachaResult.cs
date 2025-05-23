using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GachaResult : MonoBehaviour
{
    //나중에 영웅, 타워도 등급 추가됐을 때 사용
    [SerializeField] private Image resultBackground;
    [SerializeField] List<Color> rarityColors = new();

    [SerializeField] private Image resultIcon;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI nameText;

    private bool isChamp;
    private int id;

    public void Init(bool isChampion, int id)
    {
        isChamp = isChampion;
        this.id = id;

        if (isChamp)
        {
            resultIcon.sprite = Resources.Load<Sprite>($"Icons/Champion/Champion_{id}");
            typeText.text = "영웅";
            nameText.text = DataManager.Instance.championDict[id].name;
        }
        else
        {
            resultIcon.sprite = Resources.Load<Sprite>($"Icons/Tower/Tower_{id}");
            typeText.text = "타워";
            nameText.text = DataManager.Instance.towerDict[id].name;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
