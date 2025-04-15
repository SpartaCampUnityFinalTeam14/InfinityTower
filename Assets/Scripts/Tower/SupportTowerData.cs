using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportTowerData : TowerData
{
    public int suportId;
    // 버프/디버프 데이터 추가
    public int buffType;
    public float buffDuration;

    public int debuffType;
    public float debuffDuration;
    public BuffType BuffType => (BuffType)buffType;
    public DebuffType DebuffType => (DebuffType)debuffType;
}
