using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
