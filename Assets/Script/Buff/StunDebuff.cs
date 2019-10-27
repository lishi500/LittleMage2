using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunDebuff : BaseBuff
{
    private float currentAnimatorSpeed;
    //private float currentMoveSpeed;
    //private float currentAngularSpeed;

    public override void OnBuffApply()
    {
        CustomAnimationController animationController = GetAnimationController();
        if (animationController != null)
        {
            currentAnimatorSpeed = animationController.animator.speed;
            animationController.SetAnimatorSpeed(0.2f);
        }

        holder.statusManager.AddStatus(RoleStatus.STUN);
    }

   
    public override void OnBuffRemove()
    {
        CustomAnimationController animationController = GetAnimationController();
        if (animationController != null)
        {
            animationController.SetAnimatorSpeed(currentAnimatorSpeed);
        }
        holder.statusManager.RemoveStatus(RoleStatus.STUN);
    }


    public override void OnBuffEvaluated()
    {
    }

    // Update is called once per frame
    void Awake()
    {
        type = BuffType.CONTROL;
        isDeBuff = true;
    }

}
