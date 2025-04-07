using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MonsterBase : Poolable
{
    MonsterData data;
    
    public void Init(int id)
    {
        data = DataManager.Instance.monsterDict[id];
    }
}
