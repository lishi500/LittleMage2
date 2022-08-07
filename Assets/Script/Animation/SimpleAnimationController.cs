using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAnimationController : CustomAnimationController
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
                    SetBoolState("attack");
                    break;
                case AnimationState.FIGHT_STANCE:
                    SetBoolState("fight_stance");
                    break;
                case AnimationState.CAST:
                    SetBoolState("cast");
                    break;
                //case AnimationState.CHANNELING:
                case AnimationState.GET_HIT:
                    SetBoolState("get_hit");
                    break;
                case AnimationState.DIE:
                    SetBoolState("die");
                    break;
                default:
                    SetBoolState("");
                    break;
            }
            previousState = animationState;
        }
    }
}
