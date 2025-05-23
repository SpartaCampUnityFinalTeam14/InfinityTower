using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatEach : MonoBehaviour
{
    [SerializeField] private Image statIcon;
    [SerializeField] private TextMeshProUGUI statNameText;
    [SerializeField] private TextMeshProUGUI statValueText;

    public void Init(StatType type, string name, float value)
    {
        //스프라이트 세팅
        statNameText.text = name;
        statValueText.text = value.ToString();
    }
}
