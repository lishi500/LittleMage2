using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageFireBallSkillController : OrbSkillController
{
    public override void OnSkillCast()
    {
        Debug.Log("OnSkillCast MageFireBallSkillController " + skill.skillName);

    }

    public override void SkillUpdate()
    {

    }

    public override void SkillOnTrail() {

    }
}
