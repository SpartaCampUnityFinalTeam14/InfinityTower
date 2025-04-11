using UnityEngine;
using System.Collections.Generic;

public class SkillTargetingSystem : MonoBehaviour
{
    public static SkillTargetingSystem Instance;
    private TargetPositionSkill currentSkill;
    private ISkillUser currentCaster;

    private void Awake()
    {
        Instance = this;
    }

    public void StartTargeting(TargetPositionSkill skill, ISkillUser caster)
    {
        currentSkill = skill;
        currentCaster = caster;

        Debug.Log("🎯 마우스 클릭으로 위치 선택 대기");
    }

    private void Update()
    {
        if (currentSkill == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;

            currentSkill.ExecuteAt(mouseWorld, currentCaster);

            currentSkill = null;
            currentCaster = null;
        }
    }
}