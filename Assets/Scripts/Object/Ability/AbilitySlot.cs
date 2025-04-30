using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    [SerializeField] Outline outline;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Image icon;
    [SerializeField] float fadeDuration = 0.3f;
    public event Action<AbilitySlot> actionClick;
    public AbilityData Data { get; private set; }

    Image background;

    private void Awake()
    {
        button.onClick.AddListener(OnButtonClick);
        background = GetComponent<Image>();
    }

    public void Init(AbilityData data)
    {
        this.Data = data;

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
        icon.sprite = Resources.Load<Sprite>($"Icons/Ability/{Path.ChangeExtension(data.image, null)}");

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < data.valueType.Count; i++)
        {
            //sb.AppendLine($"{DataManager.Instance.abilityTypedict[data.valueType[i]].description} ({data.value[i]})");
            //sb.AppendLine($"{DataManager.Instance.abilityTypedict[data.valueType[i]].description} ({data.value[i]})");
        }
        value.text = sb.ToString();

        // 슬롯 버튼 비활성
        button.enabled = false;

        // 아웃라인 비활성
        outline.enabled = false;

        // alpha값 초기화
        canvasGroup.alpha = 1f;

        // 모든 이벤트 구독 해제
        actionClick = null;
    }

    public void EnabledButton(bool isEnabled)
    {
        button.enabled = isEnabled;
    }

    public void EnabledOutline(bool isEnabled)
    {
        outline.enabled = isEnabled;
    }

    public void FadeOut()
    {
        canvasGroup.DOFade(0f, fadeDuration).SetUpdate(true);
    }

    void OnButtonClick()
    {
        actionClick?.Invoke(this);
    }
}
