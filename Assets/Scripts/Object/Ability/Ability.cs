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
        stack = 1;
    }

    public bool TryAddStack(int num = 1)
    {
        if (data.stackLimit < stack + num)
            return false;

        stack += num;
        return true;
    }

    public void SubStack(int num = 1)
    {
        stack -= num;
    }
}
