using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChampionSelect : UI
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Image championImage;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Image Skill1Image;
    [SerializeField] private Image Skill2Image;

    List<UI_ChampionSlot> slots = new();

    public override void Clear()
    {
        
    }
}
