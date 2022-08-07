using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeSimpleAI : BaseAI
{
    public override void InProgressMonitor()
    {
    }

    private void ApproachingAttackablePosition()
    {
        aiUtils.ChasingTarget(transform, playerObj.transform, 0);
    }

    protected bool PassAttackGap()
    {
        return Time.time - lastAttackTimmer >= currentRole.GetAttackRate();
    }
    protected bool PassMaxAttackGap()
    {
        return Time.time - lastAttackTimmer >= currentRole.GetAttackRate() * currentRole.maxAttackGapFactor;
    }
    protected bool CanRangeAttack()
    {
        return PassAttackGap() &&
            aiUtils.CanAttack(gameObject, GetTargetObject(), currentRole.attackDistance, currentRole.attackIgnoreObstacle);
    }


    public override void SwitchState()
    {
        //currentState = AIState.APPROACHING;
        //ApproachingAttackablePosition();
        switch (currentState)
        {
            case AIState.IDLE:
                currentState = PassAttackGap() ? AIState.ATTACK : AIState.PATROL;
                break;
            case AIState.PATROL:
                currentState = AIState.IDLE;
                break;
            case AIState.ATTACK:

                break;
        }
        //switch (currentState)
        //{
        //    case AIState.IDLE:
        //        currentState = IsNextActionAggressive() ? AIState.APPROACHING : AIState.PATROL;
        //        break;
        //    case AIState.PATROL:
        //        currentState = IsNextActionAggressive() ? AIState.APPROACHING : AIState.PATROL;
        //        break;
        //    case AIState.APPROACHING:
        //        if (aiUtils.CanAttack(gameObject, GetTargetObject(), currentRole.attackDistance))
        //        {
        //            currentState = AIState.ATTACK;
        //        }
        //        break;
        //    case AIState.ATTACK:
        //        currentState = AIState.APPROACHING;
        //        break;
        //    case AIState.CAST:
        //        break;
        //    case AIState.FLEE:
        //        break;
        //}
    }

    public override void UpdateBehaviour()
    {
        switch (currentState)
        {
            case AIState.IDLE:
                animationController.animationState = AnimationState.IDLE;
                break;
            case AIState.PATROL:
                animationController.animationState = AnimationState.WALK;
                break;
            case AIState.APPROACHING:
                animationController.animationState = AnimationState.RUN;
                break;
            case AIState.ATTACK:
                animationController.animationState = AnimationState.ATTACK;
                break;
            case AIState.CAST:
                break;
            case AIState.FLEE:
                break;
            case AIState.REACT_HIT:
                break;
        }
    }

    // TODO fix get target, player + pet
    public override GameObject GetTargetObject()
    {
        //Debug.Log("GetTargetObject Range Simple AI");
        //if (currentRole.GetEnemyTags(). TagMapping.Player.ToString())
        //{
        //    return playerObj;
        //}

        return playerObj;
    }

    public void OnRangeAttackTrigger(string state)
    {
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