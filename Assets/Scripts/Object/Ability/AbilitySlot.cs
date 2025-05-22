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
    [SerializeField] Outline outline;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Image icon;
    [SerializeField] float fadeDuration = 0.3f;

    public event Action<AbilitySlot> actionClick;
    public AbilityData Data { get; private set; }

    private void Awake()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    public void Init(AbilityData data)
    {
        this.Data = data;

        string hex = string.Empty;
        switch (data.rarity)
        {
            case (int)Rarity.Common:
                hex = "#969696FF";
                break;
            case (int)Rarity.Rare:
                hex = "#0096FFFF";
                break;
            case (int)Rarity.Epic:
                hex = "#9600FFFF";
                break;
        }

        if (ColorUtility.TryParseHtmlString(hex, out Color color))
            outline.effectColor = color;

        title.text = data.name;
        description.text = data.description;
        icon.sprite = Resources.Load<Sprite>($"Icons/Ability/{Path.ChangeExtension(data.image, null)}");

        // 슬롯 버튼 비활성
        button.enabled = false;

        // 아웃라인
        outline.enabled = true;

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
