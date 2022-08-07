using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnhancementSkill : Skill
{
    public override void OnSkillAdd()
    {
        //GameObject gameManager = GameObject.FindGameObjectWithTag("GameController");
        //PrafabHolder prafabHolder = gameManager.GetComponent<PrafabHolder>();

        //GameObject skillHolderBuff = Instantiate(prafabHolder.SkillHolderBuff);
        //SkillHolderBuff buff = skillHolderBuff.GetComponent<SkillHolderBuff>();
        //buff.attachedSkillName = skillName;
        //buff.buffName = skillName + "-TriggerBuff";

        //owner.GetComponent<Role>().AddBuff(skillHolderBuff, owner.GetComponent<Role>());

        ApplyBuffsToRole(onApplyBuffDefs, owner.GetComponent<Role>());
    }

    public override void OnSkillCast()
    {
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
