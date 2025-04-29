using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


public abstract class BaseTower : MonoBehaviour
{
    public int ID;
    protected TowerData towerData;
    protected float attackTimer;
    // 타워가 가지고 있는 이펙트
    protected List<EffectBase> myEffect;

    // <key : 받는 이펙트의 statusID / value: 현재 적용된 이펙트 카운트> 본인이 받고있는 이펙트를 저장
    public Dictionary<int, int> nowEffectedDict;
    // 적용되는 statType의 ID 값들 , 변동되는 스탯에 대한 수치
    public Dictionary<int,float> AddModifierStat;

    protected virtual void Start()
    {
        TowerInit();
    }

    public void TowerInit()
    {
        towerData = DataManager.Instance.towerDict[ID];
        myEffect = towerData.ReturnEffectList();
        nowEffectedDict = new Dictionary<int, int>();
        AddModifierStat = new Dictionary<int, float>();

        attackTimer = towerData.GetStatValue(StatType.ActiveSpeed);
    }

    protected virtual void Update()
    {

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            Activate();
            attackTimer = towerData.GetStatValue(StatType.ActiveSpeed);
        }
    }

    // 효과 적용 후 종합 수치
    public float GetFinalStatValue(StatType statType)
    {
        return towerData.GetStatValue(statType) * (1 + GetAddModifierValue(statType));
    }

    private float GetAddModifierValue(StatType type)
    {
        if (AddModifierStat.TryGetValue((int)type,out float value))
        {
            return value;
        }
        return 0f;
    }

    protected abstract void Activate(); //실제행동은 하위 클래스에서 정의
}
