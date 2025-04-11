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
        var abilities = StageManager.Instance.abilityManager.abilities;
        Ability newAbility = new Ability();
        newAbility.Init(abilityData);

        // 이미 가지고 있는 특성일 경우 ex) 공격력 같은 공통 특성
        if (abilities.ContainsKey(abilityData.id))
        {
            for (int i = 0; i < abilityData.valueType.Count; i++)
            {
                abilities[abilityData.id].Data.value[i] += DataManager.Instance.abilityDict[abilityData.id].value[i];
            }
            abilities[abilityData.id].AddStackCount(1);
        }
        else
        {
            StageManager.Instance.abilityManager.AddAbillity(newAbility);
            abilities[abilityData.id].AddStackCount(1);
        }

        // 특성 가챠 풀에서 스택형이 아니거나 최대 스택이면 제거
        UIManager.Instance.GetUI<UIAbility>().CheckStackable(abilityData);

        // UI숨김
        UIManager.Instance.HideUI<UIAbility>();
    }
}
