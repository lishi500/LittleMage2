using ECM.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenDebuff : BaseBuff
{
    private float currentAnimatorSpeed;
    //private float currentMoveSpeed;
    //private float currentAngularSpeed;

    public override void OnBuffApply()
    {
        CustomAnimationController animationController = GetAnimationController();
        if (animationController != null) {
            currentAnimatorSpeed = animationController.animator.speed;
            animationController.SetAnimatorSpeed(0);
        }
    
        holder.statusManager.AddStatus(RoleStatus.FROZEN);
        holder.GetComponent<Rigidbody>().isKinematic = true;

        Transform model = gameObject.transform.parent.GetChild(0);
        ShaderChanger shaderChanger = model.gameObject.AddComponent<ShaderChanger>();
        shaderChanger.newShader = prafabHolder.GetShader("ice");
        shaderChanger.timeToChangeBack = duration;

        ShowEffect(OtherEffects[0], true);
        ShowEffect(OtherEffects[1], true);
    }

    public override void OnBuffEvaluated()
    {
    }

    public override void OnBuffRemove()
    {
        CustomAnimationController animationController = GetAnimationController();
        if (animationController != null)
        {
            animationController.SetAnimatorSpeed(currentAnimatorSpeed);
        }
        holder.GetComponent<Rigidbody>().isKinematic = false;
        holder.statusManager.RemoveStatus(RoleStatus.FROZEN);
    }
    void Awake()
    {
        type = BuffType.CONTROL;
    }

}
