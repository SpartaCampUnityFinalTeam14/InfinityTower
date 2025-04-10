using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TowerSelect : UI
{
    [SerializeField] private Button closeButton;
    
    [SerializeField] private List<UI_TowerSlot> slots = new();

    [SerializeField] private List<Image> selectedTowerSlots = new(5);

    public override void Clear()
    {
        
    }
}
