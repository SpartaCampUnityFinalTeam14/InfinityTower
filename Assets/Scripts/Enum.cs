using System;
using UnityEngine;

public enum Rarity
{
    Common,
    Rare,
    Epic
}

public enum TargettingRule
{
    Nearest,   // 가장 가까운 적
    Farthest,  // 가장 멀리있는 적
    LowestHP,  // 체력이 가장 낮은 적을 공격
    HighestHP, // 체력이 가장 높은 적을 공격
}

public enum TargetType
{
    Tower=1,    //아군 타워 버프
    Player,  //플레이어 코스트 회복, 체력 회복
    Enemy    //적 데미지, 디버프
}

public enum TowerStatType
{
    Damage = 0,
    Range,
    AttackSpeed,
    TargetCount,
    CoolTime,
    Cost
}

public enum EffectType
{
    // === 스탯 변경 ===
    ATKPowerUP = 1,           // 공격력 증감
    ATKSpeedUP = 2,           // 공격 속도
    RangeUP = 3,                 // 사거리
    CooltimeDown = 4,     // 스킬 쿨타임 감소

    // === 방어 관련 ===
    DefenseDown = 10,              // 방어력
    MagicResistanceDown = 11,      // 마법 저항

    // === 이동 및 행동 관련 ===
    Stun = 20,                 // 기절 (시간 동안 행동 불가)
    Slow = 21,                 // 이동 속도 감소

    // === 특수 ===
    Duration = 100,            // 지속 시간 (버프/디버프 적용 시간)
    Stackable = 101,           // 중첩 가능 여부 (별도 처리 필요 시)
    
    // === 유틸 ===
    CostRecovery = 200            // 일정 시간동안 코스트 회복속도 증가
}

public enum StatType
{
    Health,
    Speed,
    Armor,
    Attack,
    AttackSpeed,
}

public enum EnemyType
{
    Normal = 0,
    Fast = 1,
    Tank = 2,
    Boss = 3,
}

public enum SkillType
{
    AutoTarget = 0,
    TargetPosition = 1,
}


public enum EventType
{
    Choice,
    Battle,
    Penalty,
    Probablity,
    PerkChange,
    ReturnStage
}

public enum RewardType
{
    RandomRarityPerk,
    RandomCommonPerk,
    RandomRarePerk,
    RandomEpicPerk,
    Cost,
    Cooldown,
    Health = 6
}
