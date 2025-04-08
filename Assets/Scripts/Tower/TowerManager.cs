using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargettingRule
{
    Nearest,   // 가장 가까운 적
    Farthest,  // 가장 멀리있는 적
    LowestHP,  // 체력이 가장 낮은 적을 공격
    HighestHP, // 체력이 가장 높은 적을 공격
}

public enum TargetType
{
    Enemy,   //적 데미지, 디버프
    Player,  //플레이어 코스트 회복, 체력 회복
    Ally    //아군 타워 버프
}

public class TowerManager : Singleton<TowerManager>
{ 
    Dictionary<int, TowerData> TowerData;
    // Start is called before the first frame update
    void Start()
    {
        TowerDataSet();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TowerDataSet()
    {
        TowerData = DataManager.Instance.towerDict;
    }
}
