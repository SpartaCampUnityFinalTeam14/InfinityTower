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

    AbilityData data;

    private void Awake()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    public void Init(AbilityData ability)
    {
        data = ability;

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
        // 이미 가지고 있는 특성일 경우 ex) 공격력 같은 공통 특성
        if (StageManager.Instance.ability.ContainsKey(data.id))
        {
            //StageManager.Instance.ability[data.id].value += 레벨 당 증가값?
            // 추후 스택최대값도 필요 있어보임
        }
        else
        {
            StageManager.Instance.ability.Add(data.id, data);
        }

        // 특성 가챠 풀에서 스택형이 아니면 빼기
        UIManager.Instance.GetUI<UIAbility>().CheckStackable(data);

        // UI숨김
        UIManager.Instance.GetUI<UIAbility>().Hide();
    }
}
