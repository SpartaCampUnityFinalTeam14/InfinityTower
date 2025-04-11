using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_TowerSelectSlot : MonoBehaviour
{
    TowerData data;

    [SerializeField] private Button selectButton;
    [SerializeField] private Image towerImage;
    [SerializeField] private TextMeshProUGUI nameText;

    public void Init(int id)
    {
        data = DataManager.Instance.towerDict[id];

        nameText.text = data.name;
        //스프라이트 지정해줘야 함
    }
}
