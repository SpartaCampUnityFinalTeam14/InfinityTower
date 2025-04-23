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
        //attackTimer = towerData.attackSpeed;
        attackTimer = towerData.GetStatValue(TowerStatType.ActiveSpeed);
    }

    protected virtual void Update()
    {

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            Activate();
            //attackTimer = towerData.attackSpeed;
            attackTimer = towerData.GetStatValue(TowerStatType.ActiveSpeed);
        }
    }

    protected abstract void Activate(); //실제행동은 하위 클래스에서 정의
}
