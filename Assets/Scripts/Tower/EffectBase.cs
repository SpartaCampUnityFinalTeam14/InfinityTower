using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class EffectBase
{
    public int statusID;

    public EffectBase(int statusID)
    {
        this.statusID = statusID;
    }
}
public abstract class EffectBase_Tower : EffectBase
{
    protected EffectBase_Tower(int statusID) : base(statusID)
    {
    }

    public void ApplyEffect_Tower(BaseTower tarTower, float value, float duration, bool stackable)
    {
        // 효과 적용 가능 여부 판단, 타워에 효과 받음 표시
        if (tarTower.nowEffectedDict.TryGetValue(statusID, out int cnt))
        {
            if (stackable == false)
            {
                Debug.Log("이미 활성화된 이펙트, 스택 불가");
                return;
            }
            tarTower.nowEffectedDict[statusID] = cnt + 1;
        }
        else
        {
            tarTower.nowEffectedDict.Add(statusID, 1);
        }

        // 실제 효과 적용 코루틴
        tarTower.StartCoroutine(OnEffectCo_Tower(tarTower, value, duration));
    }

    private IEnumerator OnEffectCo_Tower(BaseTower tarTower, float value, float duration)
    {
        // 실제 효과 적용
        OnEffectStart_Tower(tarTower, value);

        // 적용시간 음수 시 지속시간 무한(특성)
        if (duration < 0)
        {
            yield break;
        }

        yield return new WaitForSeconds(duration);
        OnEffectEnd_Tower(tarTower, value);


        // 타워에 남은 카운트 표시 갱신
        int nowCnt = tarTower.nowEffectedDict[statusID] - 1;

        if (nowCnt == 0) tarTower.nowEffectedDict.Remove(statusID);
        else tarTower.nowEffectedDict[statusID] = nowCnt;
    }

    protected virtual void OnEffectStart_Tower(BaseTower tower, float value) { }
    protected virtual void OnEffectEnd_Tower(BaseTower tower, float value) { }
}

public abstract class EffectBase_Monster : EffectBase
{
    protected EffectBase_Monster(int statusID) : base(statusID)
    {

    }

    public void ApplyEffect_Monster(MonsterBase tarMonster, float value, float duration, bool stackable)
    {
        // 효과 적용 가능 여부 판단, 몬스터에 효과 받음 표시
        if (tarMonster.nowEffectedDict.TryGetValue(statusID, out int cnt))
        {
            if (stackable == false)
            {
                Debug.Log("이미 활성화된 이펙트, 스택 불가");
                return;
            }
            tarMonster.nowEffectedDict[statusID] = cnt + 1;
        }
        else
        {
            tarMonster.nowEffectedDict.Add(statusID, 1);
        }

        // 실제 효과 적용 코루틴
        tarMonster.StartCoroutine(OnEffectCo_Monster(tarMonster, value, duration));
    }

    private IEnumerator OnEffectCo_Monster(MonsterBase tarMonster, float value, float duration)
    {
        OnEffectStart_Monster(tarMonster, value);
        if (duration < 0)
        {
            yield break;
        }
        yield return new WaitForSeconds(duration);
        OnEffectEnd_Monster(tarMonster, value);

        //마찬가지 처리
        int nowCnt = tarMonster.nowEffectedDict[statusID] - 1;

        if (nowCnt == 0) tarMonster.nowEffectedDict.Remove(statusID);
        else tarMonster.nowEffectedDict[statusID] = nowCnt;
    }

    protected abstract void OnEffectStart_Monster(MonsterBase monster, float value);
    protected abstract void OnEffectEnd_Monster(MonsterBase monster, float value);
}

public class AttackDamageEffecter : EffectBase_Tower
{
    public AttackDamageEffecter(int statusID) : base(statusID)
    {

    }

    protected override void OnEffectStart_Tower(BaseTower tower, float value)
    {
        if (!tower.AddModifierStat.TryAdd((int)StatType.attackDamage, value))
        {
            tower.AddModifierStat[(int)StatType.attackDamage] += value;
        }
    }

    protected override void OnEffectEnd_Tower(BaseTower tower, float value)
    {
        tower.AddModifierStat[(int)StatType.attackDamage] -= value;
    }
}

public class AttackRangeEffecter : EffectBase_Tower
{
    public AttackRangeEffecter(int statusID) : base(statusID)
    {

    }

    protected override void OnEffectStart_Tower(BaseTower tower, float value)
    {
        if (!tower.AddModifierStat.TryAdd((int)StatType.attackRange, value))
        {
            tower.AddModifierStat[(int)StatType.attackRange] += value;
        }
    }

    protected override void OnEffectEnd_Tower(BaseTower tower, float value)
    {
        tower.AddModifierStat[(int)StatType.attackRange] -= value;
    }
}

public class AttackSpeedEffecter : EffectBase_Tower
{
    public AttackSpeedEffecter(int statusID) : base(statusID)
    {

    }

    protected override void OnEffectStart_Tower(BaseTower tower, float value)
    {
        if (!tower.AddModifierStat.TryAdd((int)StatType.attackSpeed, value))
        {
            tower.AddModifierStat[(int)StatType.attackSpeed] += value;
        }
    }

    protected override void OnEffectEnd_Tower(BaseTower tower, float value)
    {
        tower.AddModifierStat[(int)StatType.attackSpeed] -= value;
    }
}

public class TargetCountEffecter : EffectBase_Tower
{
    public TargetCountEffecter(int statusID) : base(statusID)
    {

    }

    protected override void OnEffectStart_Tower(BaseTower tower, float value)
    {
        if (!tower.AddModifierStat.TryAdd((int)StatType.targetCount, value))
        {
            tower.AddModifierStat[(int)StatType.targetCount] += value;
        }
    }

    protected override void OnEffectEnd_Tower(BaseTower tower, float value)
    {
        tower.AddModifierStat[(int)StatType.targetCount] -= value;
    }
}

public class TowerCooldownEffecter : EffectBase_Tower
{
    public TowerCooldownEffecter(int statusID) : base(statusID)
    {

    }

    protected override void OnEffectStart_Tower(BaseTower tower, float value)
    {
        if (!tower.AddModifierStat.TryAdd((int)StatType.towerCooldown, value))
        {
            tower.AddModifierStat[(int)StatType.towerCooldown] += value;
        }
    }

    protected override void OnEffectEnd_Tower(BaseTower tower, float value)
    {
        tower.AddModifierStat[(int)StatType.towerCooldown] -= value;
    }
}

public class CostEffecter : EffectBase_Tower
{
    public CostEffecter(int statusID) : base(statusID)
    {

    }

    protected override void OnEffectStart_Tower(BaseTower tower, float value)
    {
        if (!tower.AddModifierStat.TryAdd((int)StatType.costHeal, value))
        {
            tower.AddModifierStat[(int)StatType.costHeal] += value;
        }
    }

    protected override void OnEffectEnd_Tower(BaseTower tower, float value)
    {
        tower.AddModifierStat[(int)StatType.costHeal] -= value;
    }
}

public class HPEffecter : EffectBase_Monster
{
    public HPEffecter(int statusID) : base(statusID)
    {

    }

    protected override void OnEffectStart_Monster(MonsterBase monster, float value)
    {
        if (!monster.AddModifierStat.TryAdd((int)StatType.HP, value))
        {
            monster.AddModifierStat[(int)StatType.HP] += value;
        }
    }

    protected override void OnEffectEnd_Monster(MonsterBase monster, float value)
    {
        monster.AddModifierStat[(int)StatType.HP] -= value;
    }
}

public class MoveSpeedEffecter : EffectBase_Monster
{
    public MoveSpeedEffecter(int statusID) : base(statusID)
    {

    }

    protected override void OnEffectStart_Monster(MonsterBase monster, float value)
    {
        if (!monster.AddModifierStat.TryAdd((int)StatType.moveSpeed, value))
        {
            monster.AddModifierStat[(int)StatType.moveSpeed] += value;
        }
    }

    protected override void OnEffectEnd_Monster(MonsterBase monster, float value)
    {
        monster.AddModifierStat[(int)StatType.moveSpeed] -= value;
    }
}

public class ArmorEffecter : EffectBase_Monster
{
    public ArmorEffecter(int statusID) : base(statusID)
    {

    }

    protected override void OnEffectStart_Monster(MonsterBase monster, float value)
    {
        if (!monster.AddModifierStat.TryAdd((int)StatType.armor, value))
        {
            monster.AddModifierStat[(int)StatType.armor] += value;
        }
    }

    protected override void OnEffectEnd_Monster(MonsterBase monster, float value)
    {
        monster.AddModifierStat[(int)StatType.armor] -= value;
    }
}

public class DamageEffecter : EffectBase_Monster
{
    public DamageEffecter(int statusID) : base(statusID)
    {

    }

    protected override void OnEffectStart_Monster(MonsterBase monster, float value)
    {
        if (!monster.AddModifierStat.TryAdd((int)StatType.damage, value))
        {
            monster.AddModifierStat[(int)StatType.damage] += value;
        }
    }

    protected override void OnEffectEnd_Monster(MonsterBase monster, float value)
    {
        monster.AddModifierStat[(int)StatType.damage] -= value;
    }
}

public class CooldownEffecter : EffectBase_Monster
{
    public CooldownEffecter(int statusID) : base(statusID)
    {

    }

    protected override void OnEffectStart_Monster(MonsterBase monster, float value)
    {
        if (!monster.AddModifierStat.TryAdd((int)StatType.cooldown, value))
        {
            monster.AddModifierStat[(int)StatType.cooldown] += value;
        }
    }

    protected override void OnEffectEnd_Monster(MonsterBase monster, float value)
    {
        monster.AddModifierStat[(int)StatType.cooldown] -= value;
    }
}