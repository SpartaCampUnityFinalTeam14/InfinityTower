using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIcon : Poolable
{
    Image icon;

    private void Awake()
    {
        icon = GetComponent<Image>();
    }

    public void SetIcon(Sprite icon)
    {
        this.icon.sprite = icon;
    }
}
