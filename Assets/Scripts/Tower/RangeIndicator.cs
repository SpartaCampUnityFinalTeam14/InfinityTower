using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeIndicator : Poolable
{
    public void Init(float radius)
    {
        transform.localPosition = Vector3.zero;
        transform.localScale = new Vector2(radius * 2, radius * 2);
    }
}
