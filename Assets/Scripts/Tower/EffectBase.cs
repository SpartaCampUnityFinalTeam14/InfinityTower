using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public abstract class EffectBase
{
    public int statusID;            // 타겟 스탯 아이디
    public float value;             // 스탯 벨류
    public float duration;          // 지속 시간
    public bool stackable;          // 중첩 여부

    public abstract void ApplyEffect_Monster(MonsterBase monster);
    public abstract void ApplyEffect_Tower(BaseTower tower);

    public EffectBase(int statusID, float value, float duration, bool stackable)
    {
        this.statusID = statusID;
        this.value = value;
        this.duration = duration;
        this.stackable = stackable;
    }
}

public class AttackDamageEffecter : EffectBase
{
    public AttackDamageEffecter(int statusID, float value, float duration, bool stackable) : base(statusID, value, duration, stackable)
    {
    }

    public override void ApplyEffect_Tower(BaseTower tower)
    {
        throw new System.NotImplementedException();
    }

    public override void ApplyEffect_Monster(MonsterBase monster)
    {
        throw new System.NotImplementedException();
    }
}
