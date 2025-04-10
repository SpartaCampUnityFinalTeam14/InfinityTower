using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Gacha : UI
{
    [SerializeField] private Button closeButton;

    [SerializeField] private List<UI_ArtifactSlot> slots = new();

    [SerializeField] private Button gachaButton;
    [SerializeField] private Image resultImage;
    [SerializeField] private TextMeshProUGUI nameText;

    public override void Clear()
    {
        
    }
}
