using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAnimationController : CustomAnimationController
{
    float lastRandomTimmer;
    float lastRandomTime;
    public override void UpdateAnimationByState()
    {
        UpdateRandom100();

        if (animationState != previousState)
        {
            //Debug.Log("UpdateAnimationByState [" + previousState + " -> " + animationState + "]");
            switch (animationState)
            {
                //case AnimationState.IDLE:
                //    SetBoolState("");
                //    break;
                case AnimationState.WALK:
                    SetBoolState(AnimationState.WALK);
                    break;
                case AnimationState.RUN:
                    SetBoolState(AnimationState.RUN);
                    break;
                case AnimationState.FLY:
                    SetBoolState(AnimationState.FLY);
                    break;
                    //case AnimationState.ATTACK:
                    //    SetBoolState("attack");
                    //    break;
                    //case AnimationState.FIGHT_STANCE:
                    //    SetBoolState("fight_stance");
                    //    break;
                    //case AnimationState.CAST:
                    //    SetBoolState("cast");
                    //    break;
                    ////case AnimationState.CHANNELING:
                    //case AnimationState.GET_HIT:
                    //    SetBoolState("get_hit");
                    //    break;
                    //case AnimationState.DIE:
                    //    SetBoolState("die");
                    //    break;
                    //default:
                    //    SetBoolState("");
                    //    break;
            }
            previousState = animationState;
        }
    }

   

    private void UpdateRandom100()
    {
        if (lastRandomTime + 1 < Time.time)
        {
            lastRandomTime = Time.time;
            SetInt(AnimationState.RANDOM_100, UnityEngine.Random.Range(0, 100));
        } 
    }

}
