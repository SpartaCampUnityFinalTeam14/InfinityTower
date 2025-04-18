using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AbilityIcon : Poolable
{
    public event Action<AbilityData> clickEvent;

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
        data = null;
        button.enabled = false;
    }

    public void Init(AbilityData data, bool buttonEnabled = false)
    {
        this.data = data;

        icon.sprite = Resources.Load<Sprite>($"Prefabs/Icons/{data.image}");
        button.enabled = buttonEnabled;
    }

    public void SetButtonEnabled(bool isEnabled)
    {
        button.enabled = isEnabled;
    }

    public void OnClickButton()
    {
        clickEvent?.Invoke(data);
    }
}

