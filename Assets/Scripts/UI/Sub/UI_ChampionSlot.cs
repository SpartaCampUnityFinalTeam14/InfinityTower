using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChampionSlot : MonoBehaviour
{
    ChampionData data;

    [SerializeField] private Button selectButton;
    [SerializeField] private Image championImage;

    [SerializeField] private TextMeshProUGUI nameText;

    public void Init(int id)
    {
        data = DataManager.Instance.championDict[id];

        nameText.text = data.name;
        //아이디에 맞춰서 스프라이트 찾아와야 함
    }
}
