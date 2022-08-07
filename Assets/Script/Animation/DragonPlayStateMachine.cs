using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonPlayStateMachine : StateMachineBehaviour
{
    DragonEventHelper dragonEventHelper;
    string END = "End";
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    Debug.Log("OnStateExit " + animator.GetCurrentAnimatorClipInfo(layerIndex)[0].clip.name);
    //   
    //}
    string enterStateName;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("OnStateEnter " + animator.GetCurrentAnimatorClipInfo(layerIndex)[0].clip.name + " " + Time.time);
        enterStateName = animator.GetCurrentAnimatorClipInfo(layerIndex)[0].clip.name;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("OnStateExit " + animator.GetCurrentAnimatorClipInfo(layerIndex)[0].clip.name + " " + Time.time);

        if (enterStateName != animator.GetCurrentAnimatorClipInfo(layerIndex)[0].clip.name) {

            DragonEventHelper dragonEventHelper = animator.gameObject.GetComponent<DragonEventHelper>();
            if (dragonEventHelper != null)
            {
                dragonEventHelper.OnPlayTrigger(END);
            }
        }
    }

  
    void Start() {

    }

}
