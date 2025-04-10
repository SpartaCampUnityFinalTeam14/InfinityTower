using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


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
            cooldownTimer = towerData.coolTime;
            Activate();
        }
    }

    public abstract void Activate(); //실제행동은 하위 클래스에서 정의


}
