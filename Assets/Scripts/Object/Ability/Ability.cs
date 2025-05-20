using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability
{
    AbilityData data;
    public AbilityData Data => data;

    int stack;
    public int CurStack => stack;

    public Ability(AbilityData data)
    {
        Init(data);
    }

    private void Init(AbilityData data)
    {
        this.data = data.DeepCopy();
        stack = 0;
    }

    public void AddStack(int cnt = 1)
    {
        stack += cnt;
    }

    public void SubStack(int cnt = 1)
    {
        stack -= cnt;
    }
}
