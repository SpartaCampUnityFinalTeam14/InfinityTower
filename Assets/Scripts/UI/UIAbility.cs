using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Playables;
using UnityEngine;



public class UIAbility : UI
{
    [SerializeField] Transform layout;

    List<AbilitySlot> slots;
    const string prefabPath = "Ability/AbilitySlot";

    protected override void Awake()
    {
        base.Awake();
        slots = new List<AbilitySlot>();
    }

    public override void Show()
    {
        base.Show();
        Time.timeScale = 0f;
    }

    public override void Hide()
    {
        base.Hide();
        Time.timeScale = 1f;
        StageManager.Instance.CurFloor.isPerkSelected = true;
    }

    public void DrawAbility(int drawCount = 3)
    {
        AbilityData data;
        List<AbilityData> listDraw = new List<AbilityData>();
        int maxIdx = 0;
        while (listDraw.Count < drawCount)
        {
            // 레어도
            int rarity = GetRandomRarity();

            // 특성 뽑기
            data = GetRandomAbility(rarity);

            if (data == null) continue;

            // 이미 뽑은 특성인지 중복 체크
            if (!listDraw.Contains(data))
                listDraw.Add(data);

            maxIdx++;
            if (maxIdx > 50)
                break;
        }

        // 특성 생성
        for (int i = 0; i < listDraw.Count; i++)
        {
            CreateAbilitySlot(listDraw[i], i);
        }
    }

    public void CheckStackable(AbilityData data)
    {
        if (data.stackable <= 0 || DataManager.Instance.abilityDict[data.id].maxStack <= )
            StageManager.Instance.filterAbilityPool[data.rarity].Remove(data.id);
    }

    int GetRandomRarity()
    {
        // 1성 60%, 2성 30%, 3성 10%
        float roll = Random.value;

        if (roll < 0.6f)
            return (int)Rarity.Common;
        else if (roll < 0.9f)
            return (int)Rarity.Rare;
        else
            return (int)Rarity.Epic;
    }

    AbilityData GetRandomAbility(int rarity)
    {
        var abilityDatas = StageManager.Instance.filterAbilityPool[rarity].Values.ToList();
        
        return abilityDatas.Count < 1 ? null : abilityDatas[Random.Range(0, abilityDatas.Count)];
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
