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

        StageManager.Instance.timeScaleManager.PushTimeScale(0f);

        // 특성 리스트 업데이트
        UpdateAbilityIcon();
    }

    public override void Hide()
    {
        base.Hide();

        if (StageManager.Instance)
            StageManager.Instance.timeScaleManager.PopTimeScale();

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
        var list = StageManager.Instance.abilityManager.curAbilities;
        GameObject prefab = Resources.Load<GameObject>(iconPrefabPath);

        foreach (var ability in list.Values)
        {
            AbilityIcon icon = PoolManager.Instance.Get(prefab, 20, content).GetComponent<AbilityIcon>();
            icon.Init(ability.Data);

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
