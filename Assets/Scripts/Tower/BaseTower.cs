using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


public abstract class BaseTower : MonoBehaviour
{
    public TowerData towerData;
    protected float attackTimer;

    protected virtual void Start()
    {
        attackTimer = 1f / towerData.attackSpeed;
    }

    protected virtual void Update()
    {

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            Activate();
            attackTimer = 1f / towerData.attackSpeed;
        }
    }

    protected abstract void Activate(); //실제행동은 하위 클래스에서 정의

    public virtual float GetRange()
    {
        return towerData.range;
    }

    public virtual TargetType GetTargetType()
    {
        return towerData.TargetType;
    }
}
