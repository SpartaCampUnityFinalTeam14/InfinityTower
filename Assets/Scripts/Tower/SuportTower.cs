using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuportTower : TargettingTower
{
    protected override void UseActOnTargets()
    {
        foreach (var target in targets)
        {
            // 적 디버프 or 아군 버프 부여
        }
    }
}

