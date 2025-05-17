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
    Tower,    //아군 타워 버프
    Player,  //플레이어 코스트 회복, 체력 회복
    Enemy    //적 데미지, 디버프
}

public enum StatType
{
    //Tower
    attackDamage = 0,
    attackRange = 1,
    attackSpeed = 2,
    targetCount = 3,
    towerCooldown = 4,
    cost = 5,

    //Player
    costHeal = 6,
    playerHeal = 7,
    playerHP = 8,

    //Monster
    HP = 9,
    moveSpeed = 10,
    armor = 11,
    damage = 12,
    cooldown = 13,

    //Player
    atk = 14,
    startCost = 15,
    cleargoldDrop = 16
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
