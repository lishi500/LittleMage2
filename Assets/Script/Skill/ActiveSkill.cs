using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkill : Skill
{
    [Header("ActiveSkill")]
    public ReactEventType triggerType = ReactEventType.ATTACK;

    public override void OnSkillAdd()
    {
        //GameObject gameManager = GameObject.FindGameObjectWithTag("GameController");
        //PrafabHolder prafabHolder = gameManager.GetComponent<PrafabHolder>();

        //GameObject skillHolderBuff = Instantiate(prafabHolder.SkillHolderBuff);
        //SkillHolderBuff buff = skillHolderBuff.GetComponent<SkillHolderBuff>();
        //buff.reactTypes.Add(triggerType);
        //buff.attachedSkillName = skillName;
        //buff.buffName = skillName + "-TriggerBuff";

        //owner.GetComponent<Role>().AddBuff(skillHolderBuff, owner.GetComponent<Role>());

    }

    public override void OnSkillCast()
    {
    }

    public override void SkillSetup()
    {
    }

    public  override void UpdateCollider()
    {
    }

    public override void UpdateEffect()
    {
    }

    // Start is called before the first frame update
    void Awake()
    {
    }
}
