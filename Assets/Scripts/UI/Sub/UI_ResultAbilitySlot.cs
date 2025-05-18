using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ResultAbilitySlot : MonoBehaviour
{
    public int id;

    [SerializeField] private Image abilityIcon;
    [SerializeField] private TextMeshProUGUI nameText;

    public void Init(int id, int count)
    {
        this.id = id;

        var data = DataManager.Instance.abilityDict[id];
        nameText.text = data.name;
        abilityIcon.sprite = Resources.Load<Sprite>($"Icons/Ability/{data.image}");
    }
}
