using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySlot : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI value;
    Image background;

    AbilityData abilityData;

    private void Awake()
    {
        button.onClick.AddListener(OnButtonClick);
        background = GetComponent<Image>();
    }

    public void Init(AbilityData data)
    {
        this.abilityData = data;

        switch (data.rarity)
        {
            case (int)Rarity.Common:
                background.color = Color.white;
                break;
            case (int)Rarity.Rare:
                background.color = Color.blue;
                break;
            case (int)Rarity.Epic:
                background.color = Color.magenta;
                break;
        }

        title.text = data.name;
        description.text = data.description;

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < data.valueType.Count; i++)
        {
            sb.AppendLine($"{DataManager.Instance.abilityTypedict[data.valueType[i]].description} ({data.value[i]})");
        }
        value.text = sb.ToString();
    }

    void OnButtonClick()
    {
        StageManager.Instance.abilityManager.AddAbillity(abilityData);

        // UI숨김
        UIManager.Instance.HideUI<UIAbility>();
    }
}
