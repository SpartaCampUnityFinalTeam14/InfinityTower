using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


public abstract class BaseTower : MonoBehaviour
{
    public int ID;
    protected TowerData towerData;
    protected float attackTimer;
    protected List<EffectBase> myEffect;

    protected virtual void Start()
    {
        towerData = DataManager.Instance.towerDict[ID];
        myEffect = towerData.ReturnEffectList();
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
