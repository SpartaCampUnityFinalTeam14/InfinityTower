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

public enum BuffType
{
    AttackSpeed,
    Range,
    CooldownReduction
}
public enum DebuffType
{
    Slow,
    DefenseDown
}
