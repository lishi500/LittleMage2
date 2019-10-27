using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStrick : ActiveSkill
{
    public override void OnSkillCast()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameController");
        PrafabHolder prafabHolder = gameManager.GetComponent<PrafabHolder>();
        GameObject lightningPrafab = prafabHolder.GetEffect("LightningArc");
        SkillController skillController = skillControllerObj.GetComponent<SkillController>();

        GameObject lightningArc = Instantiate(lightningPrafab);
        LightningLink[] links = lightningPrafab.GetComponents<LightningLink>();

        foreach (LightningLink link in links)
        {
            link.m_StartPoint = skillController.GeneratePositionByType(PositionType.SELF);
            link.m_EndPoint = skillController.GeneratePositionByType(PositionType.TARGET);
        }

        lightningArc.AddComponent<AutoDestroy>().timeToLive = duration;
        Role role = skillController.GeneratePositionByType(PositionType.TARGET).GetComponent<Role>();
        if (role != null) {
            role.ReduceHealth(CalculateValue());
            ApplyBuffsToRole(onApplyBuffDefs, role);
        }
    }
    public override void SkillSetup()
    {
       

    }



    public override void UpdateCollider()
    {
    }

    public override void UpdateEffect()
    {
    }

  
}
