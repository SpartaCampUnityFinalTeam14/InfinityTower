using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class UIPause : UI
{
    [SerializeField] Transform content;
    [SerializeField] Button btnResume;
    [SerializeField] GameObject slotPanel;
    [SerializeField] AbilitySlot abilitSlot;
    [SerializeField] Button btnAbilityClose;

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

        //if (StageManager.Instance)
        //    StageManager.Instance.timeScaleManager.PopTimeScale();

        ReleaseAbilityIcon();
    }

    public void TogglePause()
    {
        if (!isToggle)
            Show();
        else
        {
            StageManager.Instance.timeScaleManager.PopTimeScale();

            Hide();
        }

        isToggle = !isToggle;
    }

    void UpdateAbilityIcon()
    {
        var list = StageManager.Instance.abilityManager.allAbilities;
        GameObject prefab = Resources.Load<GameObject>(iconPrefabPath);

        foreach (var ability in list.Values)
        {
            AbilityIcon icon = PoolManager.Instance.Get(prefab, 20, content).GetComponent<AbilityIcon>();
            icon.Init(ability.Data, true);
            icon.clickEvent2 += ShowAbilityInfo;

            listIcon.Add(icon);
        }

        slotPanel.SetActive(false);
        abilitSlot.gameObject.SetActive(false);
    }

    void ReleaseAbilityIcon()
    {
        foreach (var icon in listIcon)
        {
            PoolManager.Instance.Release(icon);
        }

        listIcon.Clear();
    }

    void ShowAbilityInfo(AbilityData data, Transform transform)
    {
        abilitSlot.Init(data);

        slotPanel.SetActive(true);
        btnAbilityClose.enabled = true;

        abilitSlot.gameObject.SetActive(true);

        abilitSlot.transform.localScale = Vector3.zero;
        abilitSlot.transform.position = new Vector2(transform.position.x, abilitSlot.transform.position.y);

        abilitSlot.transform.DOScale(1f, 0.5f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    public void CloseAbility()
    {
        btnAbilityClose.enabled = false;

        abilitSlot.transform.DOScale(0f, 0.5f)
            .SetEase(Ease.OutQuart)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                slotPanel.SetActive(false);
                abilitSlot.gameObject.SetActive(false);
            });
    }
}
