using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GachaResult : MonoBehaviour
{
    [SerializeField] private Image resultBackground;
    [SerializeField] List<Color> rarityColors = new();
    [SerializeField] private Image resultIcon;
    [SerializeField] private TextMeshProUGUI nameText;
}
