using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete]
public class SkillHolderBuff : BaseBuff
{
    private Skill skill;
    private PlayerController playerController;

    [Obsolete]
    public override void OnAttack(string state)
    {
        if (reactTypes.Contains(ReactEventType.ATTACK)) {
            if (state == "Trigger")
            {
                if (playerController.CanCastSkill())
                {
                    //Role role = holder;
                    //Skill skill = role.GetSkillByName(attachedSkillName);
                    //Debug.Log("SkillHolderBuff -> On Attack triggered " + skill.skillName + " isReady -> " + skill.IsReady() + " " + skill.CDLeft);

                    if (skill != null && skill.IsReady())
                    {
                        playerController.CastSkill(skill);
                        //Debug.Log("SkillHolderBuff -> Cast skill " + skill.skillName);
                    }
                }

            }
        }
    }

    public virtual void SkillReady() {

        if (skill != null && skill.IsReady() && playerController.CanCastSkill()) {
            if (skill.needTarget && playerController.target != null)
            {
                playerController.CastSkill(skill);
            }
            else if (!skill.needTarget) {
                playerController.CastSkill(skill);
            }
        }
    }

    public override void OnBuffApply()
    {
    }

    public override void OnBuffTrigger()
    {
        //Debug.Log("OnBuffEvaluated");
        if (reactTypes.Contains(ReactEventType.SKILL_READY)) {
            //Debug.Log("OnBuffEvaluated SkillReady");

            SkillReady();
        }
    }

    public override void OnBuffRemove()
    {
    }


    public override void Start()
    {
        base.Start();
      
        skill = holder.GetSkillByName(attachedSkillName);
        playerController = GetComponentInParent<PlayerController>();
    }
    private void Awake()
    {
        if (reactTypes == null)
        {
            reactTypes = new List<ReactEventType>();
        }
    }

    public override BuffEvaluatorResult OnBuffEvaluated(BuffEvaluatorResult evaluatorResult)
    {
        return evaluatorResult;
    }

}
