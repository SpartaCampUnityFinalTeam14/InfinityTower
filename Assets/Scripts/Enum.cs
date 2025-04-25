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

public enum BuffEffectType
{
    Damage = 0,                // 공격력
    // === 스탯 변경 ===
    AttackPower = 1,           // 공격력 증감
    AttackSpeed = 2,           // 공격 속도
    Range = 3,                 // 사거리
    CooldownReduction = 4,     // 스킬 쿨타임 감소

    // === 방어 관련 ===
    Defense = 10,              // 방어력
    MagicResistance = 11,      // 마법 저항
    Shield = 12,               // 일시적인 보호막
    DefenseDown = 13,

    // === 이동 및 행동 관련 ===
    MoveSpeed = 20,            // 이동 속도
    Stun = 21,                 // 기절 (시간 동안 행동 불가)
    Slow = 22,                 // 이동 속도 감소
    Silence = 23,              // 스킬 사용 불가

    // === 체력 및 회복 ===
    HealOverTime = 30,         // 초당 체력 회복
    InstantHeal = 31,          // 즉시 체력 회복
    LifeSteal = 32,            // 흡혈

    // === 기타 ===
    CritChance = 40,           // 치명타 확률
    CritDamage = 41,           // 치명타 피해
    Evasion = 42,              // 회피 확률
    Accuracy = 43,             // 명중률

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
