using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Gacha : UI
{
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private Animator boxAnimator;
    [SerializeField] private Button gacha1Button;
    [SerializeField] private Button gacha10Button;
    
    [SerializeField] private GameObject gachaEachBackground;
    [SerializeField] private Button gachaEachBackgroundButton;
    private List<int> gachaList = new();
    [SerializeField] private UI_GachaResult gachaEachResult;

    [SerializeField] private GameObject gachaAllBackground;
    [SerializeField] private Button gachaAllBackgroundButton;
    [SerializeField] private List<UI_GachaResult> gachaAllResult;
}
