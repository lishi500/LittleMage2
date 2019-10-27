using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbSkillController : SkillController
{
    public ContactPoint primaryContactPoint;
    public override void InitialSkill() {
        pastTime = 0;
        effectChainIndex = 0;
        colliderChainIndex = 0;
        skill.SkillSetup();

        skill.OnSkillCast();
        OnSkillCast();
        isStarted = true;
    }

}
