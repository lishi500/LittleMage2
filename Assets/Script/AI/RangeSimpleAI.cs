using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeSimpleAI : BaseAI
{
    public override void InProgressMonitor()
    {
    }

    public override void SwitchState()
    {
        switch (currentState)
        {
            case AIState.IDLE:
                currentState = IsNextActionAggressive() ? AIState.APPROACHING : AIState.PATROL;
                break;
            case AIState.PATROL:
                currentState = IsNextActionAggressive() ? AIState.APPROACHING : AIState.PATROL;
                break;
            case AIState.APPROACHING:
                if (aiUtils.CanAttack(gameObject, GetTargetObject(), currentRole.attackDistance))
                {
                    currentState = AIState.ATTACK;
                }
                break;
            case AIState.ATTACK:
                currentState = AIState.APPROACHING;
                break;
            case AIState.CAST:
                break;
            case AIState.FLEE:
                break;
        }
    }

    public override void UpdateBehaviour()
    {
    }

    public void OnRangeAttackTrigger(string state) {
    }

    public virtual void RegisterAnimationEvent()
    {
        eventHelper.notifyMeleeAttack += OnRangeAttackTrigger;
    }

    void Start()
    {
        RegisterAnimationEvent();
        updateFrequency = 0.0f;
    }
}
