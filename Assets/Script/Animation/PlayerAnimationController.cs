using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : CustomAnimationController
{
    public override void UpdateAnimationByState()
    {
        if (animationState != previousState)
        {
            //Debug.Log("UpdateAnimationByState [" + previousState + " -> " + animationState + "]");
            switch (animationState)
            {
                case AnimationState.IDLE:
                    SetBoolState("");
                    break;
                case AnimationState.WALK:
                    SetBoolState("walk");
                    break;
                case AnimationState.RUN:
                    SetBoolState("run");
                    break;
                case AnimationState.ATTACK:
                    SetAllFalse();
                    break;
                case AnimationState.ATTACK1:
                    //SetBoolState("fight_stance");
                    SetAllFalse();
                    SetAttackCount(0);
                    break;
                case AnimationState.ATTACK2:
                    //SetBoolState("fight_stance");
                    SetAllFalse();
                    SetAttackCount(1);
                    break;
                case AnimationState.ATTACK3:
                    SetAllFalse();
                    SetAttackCount(2);
                    break;
                case AnimationState.FIGHT_STANCE:
                    SetBoolState("fight_stance");
                    break;
                case AnimationState.CAST:
                    SetBoolState("cast");
                    break;
                case AnimationState.CHANNELING:

                default:
                    SetBoolState("");
                    break;
            }
            previousState = animationState;
        }
    }

    public void SetAttackCount(int attackCount) {
        SetInt("attack", attackCount);
    }
    public override void Start()
    {
         animationStates = new string[]
            { "walk", "run", "fight_stance", "defence", "get_hit", "die", "cast", "channeling", "channeling_loop" };
        AnimationClip attackClip = GetAnimationClipByName("Frank_RPG_Mage_Combo04_1");
        if (attackClip != null)
        {
            attackClipLength = attackClip.length;
        }
    }
}
