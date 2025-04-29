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

public enum TargetStatType
{
    //타워 스탯
    attackDamage = 0,
    attackRange = 1,
    attackSpeed = 2,
    targetCount = 3,
    towerCooldown = 4,
    cost = 5,

    //플레이어 스탯
    costHeal = 6,
    playerHeal = 7,
    playerHP = 8,
    
    //몬스터 스탯
    HP = 9,
    moveSpeed = 10,
    armor = 11,
    damage = 12,

    //플레이어 스탯
    cooldown = 13,
    atk = 14,
    cash = 15,
    cleargoldDrop = 16
}

public enum StatType
{
    //Tower
    Damage,
    Range,
    ActiveSpeed,
    TargetCount,
    CoolTime,
    Cost,

    //Moster
    Health,
    Speed,
    Armor,
    Attack,
    AttackSpeed
}

public enum EffectType
{
    Slow,
    Burn,
    GetCost,
    DamageBuff,
    AttackSpeedBuff,
    DefenseDown
}

public enum EnemyType
{
    Normal = 0,
    Fast = 1,
    Tank = 2,
    Boss = 3,
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
