using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public enum TargetingRule
{
    Nearest,   // 가장 가까운 적
    Farthest,  // 가장 멀리있는 적
    LowestHP,  // 체력이 가장 낮은 적을 공격
    HighestHP, // 체력이 가장 높은 적을 공격
}

public enum targetType
{
    Enemy,   //적 데미지, 디버프
    Player,  //플레이어 코스트 회복, 체력 회복
    Tower    //아군 타워 버프
}

public abstract class BaseTower : MonoBehaviour
{
    public TowerData towerData;
    public float cooldownTimer;

    public virtual void Initialize(TowerData data)
    {
        towerData = data;
        cooldownTimer = 0f;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0)
        {
            Activate();
            cooldownTimer = towerData.coolTime;
        }
    }
    public abstract void Activate(); //실제행동은 하위 클래스에서 정의


}
