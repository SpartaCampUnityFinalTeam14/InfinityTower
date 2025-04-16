using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChampionInfo : UI
{
    [Header("배경")]
    [SerializeField] private Button backgroundButton;

    [Header("좌측")]
    [SerializeField] private TextMeshProUGUI championNameText;
    [SerializeField] private Image championIcon;
    [SerializeField] private Image expBar;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private Button levelupButton;

    [Header("우측 상단")]
    [SerializeField] private TextMeshProUGUI championHPText;

    [Header("좌측 상단")]
    [SerializeField] private Image skill1Icon;
    [SerializeField] private Button skill1Button;
    [SerializeField] private Image skill2Icon;
    [SerializeField] private Button skill2Button;
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillInfoText;
    [SerializeField] private TextMeshProUGUI skillDamageText;

    [Header("하단")]
    [SerializeField] private Button selectButton;
    [SerializeField] private Button closeButton;

    public override void Clear()
    {
        base.Clear();
    }
}
