using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Playables;
using UnityEngine;



public class UIAbility : UI
{
    [SerializeField] Transform layout;
    [SerializeField] int abilitySlotCount;

    List<AbilitySlot> slots;
    const string prefabPath = "Ability/AbilitySlot";

    protected override void Awake()
    {
        base.Awake();
        slots = new List<AbilitySlot>();

        for (int i = 0; i < abilitySlotCount; i++)
        {
            Util.InstantiatePrefab(prefabPath, Vector3.zero, Quaternion.identity, layout).TryGetComponent(out AbilitySlot ability);
            slots.Add(ability);
        }
    }

    public override void Show()
    {
        base.Show();

        StageManager.Instance.timeScaleManager.PushTimeScale(0f);

        StageManager.Instance.CurFloor.isPerkSelected = false;
    }

    public override void Hide()
    {
        base.Hide();

        StageManager.Instance.timeScaleManager.PopTimeScale();

        StageManager.Instance.CurFloor.isPerkSelected = true;
    }

    public void DrawAbility(int drawCount = 3)
    {
        AbilityData data;
        List<AbilityData> listDraw = new List<AbilityData>();
        
        while (listDraw.Count < drawCount)
        {
            // 레어도
            int rarity = StageManager.Instance.abilityManager.GetRandomRarity();

            // 특성 뽑기
            data = StageManager.Instance.abilityManager.GetRandomAbility(rarity);

            if (data == null) continue;

            // 이미 뽑은 특성인지 중복 체크
            if (!listDraw.Contains(data))
                listDraw.Add(data);
        }

        // 특성 생성
        for (int i = 0; i < listDraw.Count; i++)
        {
            CreateAbilitySlot(listDraw[i], i);
        }
    }

    void CreateAbilitySlot(AbilityData data, int slotIdx)
    {
        AbilitySlot ability;
        if (slots.Count <= slotIdx)
        {
            Util.InstantiatePrefab(prefabPath, Vector3.zero, Quaternion.identity, layout).TryGetComponent(out ability);
            slots.Add(ability);
        }
        else
        {
            ability = slots[slotIdx];
        }

        ability.Init(data);
    }
}
