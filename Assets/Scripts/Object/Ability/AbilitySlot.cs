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

    AbilityData data;

    private void Awake()
    {
        button.onClick.AddListener(OnButtonClick);
        background = GetComponent<Image>();
    }

    public void Init(AbilityData ability)
    {
        data = ability;

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
        var abilities = StageManager.Instance.abilityManager.abilities;

        // 이미 가지고 있는 특성일 경우 ex) 공격력 같은 공통 특성
        if (abilities.ContainsKey(data.id))
        {
            for (int i = 0; i < data.valueType.Count; i++)
            {
                abilities[data.id].value[i] += DataManager.Instance.abilityDict[data.id].value[i];
            }
            abilities[data.id].maxStack++;
        }
        else
        {
            abilities.Add(data.id, data);
        }

        // 특성 가챠 풀에서 스택형이 아니거나 최대 스택이면 제거
        UIManager.Instance.GetUI<UIAbility>().CheckStackable(data);

        // UI숨김
        UIManager.Instance.HideUI<UIAbility>();
    }
}
