using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class UtilityTower : BaseTower
{
    private float goldGainAmount;

    public override void Initialize(TowerData data)
    {
        base.Initialize(data);
        goldGainAmount = data.GetValue(BuffEffectType.GainGold);
    }

    public override void Activate()
    {
        //골드를 더해주는 함수 추가
        Debug.Log($"[UtilityTower] 골드 +{goldGainAmount}");
    }
}
