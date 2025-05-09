using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEntryUI : UI
{
    [SerializeField] HeroSkillPanel skillPanel;
    public HeroSkillPanel SkillPanel { get; private set; }

    private void Start()
    {
        StageManager.Instance.InitHero(skillPanel);
    }
}
