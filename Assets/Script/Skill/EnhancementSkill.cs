using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnhancementSkill : Skill
{
    public override void OnSkillAd()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameController");
        PrafabHolder prafabHolder = gameManager.GetComponent<PrafabHolder>();

        GameObject skillHolderBuff = Instantiate(prafabHolder.SkillHolderBuff);
        SkillHolderBuff buff = skillHolderBuff.GetComponent<SkillHolderBuff>();
        buff.attachedSkillName = skillName;
        buff.buffName = skillName + "-TriggerBuff";

        owner.GetComponent<Role>().AddBuff(skillHolderBuff, owner.GetComponent<Role>());
    }

    public override void OnSkillCast()
    {
    }

    public override void SkillSetup()
    {
        ApplyBuffsToRole(onApplyBuffDefs, owner.GetComponent<Role>());
    }

    public override void UpdateCollider()
    {
    }

    public override void UpdateEffect()
    {
    }
}
