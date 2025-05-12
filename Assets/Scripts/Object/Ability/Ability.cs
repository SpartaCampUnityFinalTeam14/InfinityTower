using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability
{
    AbilityData data;
    public AbilityData Data => data;

    int curStackCount;
    public int CurStackCount => curStackCount;

    public Ability(AbilityData data)
    {
        Init(data);
    }

    private void Init(AbilityData data)
    {
        this.data = data.DeepCopy();
        curStackCount = 0;
    }

    public void AddStackCount(int cnt)
    {
        curStackCount += cnt;
    }

    public void SubStackCount(int cnt)
    {
        curStackCount -= cnt;
    }
}
