using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleManager
{
    Stack<float> stackTimeScale = new Stack<float>();

    public void PushTimeScale(float value)
    {
        stackTimeScale.Push(Time.timeScale);
        Time.timeScale = value;
    }

    public void PopTimeScale()
    {
        if (stackTimeScale.Count > 0)
        {
            Time.timeScale = stackTimeScale.Pop();
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
