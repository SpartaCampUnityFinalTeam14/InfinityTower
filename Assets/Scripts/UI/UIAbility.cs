using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Playables;
using UnityEngine;



public class UIAbility : UI
{
    [SerializeField] Transform layout;

    List<AbilitySlot> slots;

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

    public void DrawAbility(int count = 3)
    {
        AbilityData data;

        for (int i = 0; i < count; i++)
        {
            int rarity = GetRandomRarity();
            data = GetRandomAbility(rarity);

            // 働失 持失
            AbilitySlot ability;
            if (slots.Count <= i)
            {
                Util.InstantiatePrefab($"Ability/AbilitySlot", Vector3.zero, Quaternion.identity, layout).TryGetComponent(out ability);
                slots.Add(ability);
            }
            else
            {
                ability = slots[i];
            }

            ability.Init(data);
        }
    }

    public void CheckStackable(AbilityData data)
    {
        if (data.stackable <= 0)
            StageManager.Instance.filterAbilityPool[data.rarity].Remove(data.id);
    }

    int GetRandomRarity()
    {
        // 1失 60%, 2失 30%, 3失 10%
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
        var AbilityDatas = StageManager.Instance.filterAbilityPool[rarity].Values.ToList();

        return AbilityDatas[Random.Range(0, AbilityDatas.Count)];
    }

    
}
