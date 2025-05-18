using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AbilityIcon : Poolable
{
    public event Action<AbilityData> clickEvent;
    public event Action<AbilityData, Transform> clickEvent2;

    Image icon;
    Button button;
    AbilityData data;

    private void Awake()
    {
        icon = GetComponent<Image>();
        button = GetComponent<Button>();
    }

    private void OnDisable()
    {
        clickEvent = null;
        clickEvent2 = null;
        data = null;
        button.enabled = false;
    }

    public void Init(AbilityData data, bool buttonEnabled = false)
    {
        this.data = data;

        //string test = $"Icons/Ability/{Path.ChangeExtension(data.image, null)}";

        icon.sprite = Resources.Load<Sprite>($"Icons/Ability/{data.image}");
        button.enabled = buttonEnabled;
    }

    public void SetButtonEnabled(bool isEnabled)
    {
        button.enabled = isEnabled;
    }

    public void OnClickButton()
    {
        clickEvent?.Invoke(data);
        clickEvent2?.Invoke(data, transform);
    }
}

