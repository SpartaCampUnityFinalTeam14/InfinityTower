using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class UIPause : UI
{
    [SerializeField] Transform content;
    [SerializeField] Button btnResume;

    bool isToggle;
    List<AbilityIcon> listIcon = new List<AbilityIcon>();
    const string iconPrefabPath = "Prefabs/Ability/AbilityIcon";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause && !isToggle)
        {
            Show();
            isToggle = true;
        }
    }

    public override void Show()
    {
        base.Show();
        Time.timeScale = 0f;
        StageManager.Instance.isPause = true;

        // 특성 리스트 업데이트
        UpdateAbilityIcon();
    }

    public override void Hide()
    {
        base.Hide();

        //if (StageManager.Instance.isEventEnd && StageManager.Instance.CurFloor.isPerkSelected)
            Time.timeScale = 1f;

        StageManager.Instance.isPause = false;

        ReleaseAbilityIcon();
    }

    public void TogglePause()
    {
        if (!isToggle)
            Show();
        else
            Hide();

        isToggle = !isToggle;
    }

    void UpdateAbilityIcon()
    {
        var list = StageManager.Instance.abilityManager.abilities;
        GameObject prefab = Resources.Load<GameObject>(iconPrefabPath);

        foreach (var ability in list)
        {
            AbilityIcon icon = PoolManager.Instance.Get(prefab, 10, content).GetComponent<AbilityIcon>();
            //icon.SetIcon(Resources.Load<Sprite>($"Prefabs/Icon/ability.iconImage"));
            
            listIcon.Add(icon);
        }
    }

    void ReleaseAbilityIcon()
    {
        foreach (var icon in listIcon)
        {
            PoolManager.Instance.Release(icon);
        }

        listIcon.Clear();
    }
}
