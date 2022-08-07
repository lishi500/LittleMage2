using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAI : BaseAI
{
    
    public override void SwitchState()
    {
        //Debug.Log("Switch state current -> " + currentState);
        switch (currentState) {
            case AIState.IDLE:
                 currentState = IsNextActionAggressive() ? AIState.APPROACHING : AIState.PATROL;
                //Debug.Log("Switch from IDLE to " + currentState);
                break;
            case AIState.PATROL:
                currentState = IsNextActionAggressive() ? AIState.APPROACHING : AIState.PATROL;
                //Debug.Log("Switch from PATROL to " + currentState);
                break;
            case AIState.APPROACHING:
                if (aiUtils.CanAttack(gameObject, GetTargetObject(), currentRole.attackDistance)) {
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
            case AIState.REACT_HIT:
                if (aiUtils.CanAttack(gameObject, GetTargetObject(), currentRole.attackDistance))
                {
                    currentState = AIState.ATTACK;
                }
                else {
                    currentState = AIState.IDLE;
                }
                break;
        }
    }


    public override void UpdateBehaviour()
    {
        switch (currentState)
        {
            case AIState.IDLE:
                animationController.animationState = AnimationState.IDLE;
                break;
            case AIState.PATROL:
                PatrolAction();
                break;
            case AIState.APPROACHING:
                ApproachingAction();
                break;
            case AIState.ATTACK:
                AttackAction();
                break;
            case AIState.CAST:
                break;
            case AIState.FLEE:
                break;
            case AIState.REACT_HIT:
                ReactHitAction();
                break;
        }
    }

    public override void InProgressMonitor()
    {
        switch (currentState)
        {
            case AIState.APPROACHING:
                if (aiUtils.HasReachDestination(aiUtils.GetNavMeshAgent(gameObject.transform))) {
                    progress = AIProgress.END;
                }
                break;
            default:
                break;
        }
    }

    // TODO fix, player + pet
    public override GameObject GetTargetObject()
    {
        //Debug.Log("GetTargetObject Simple AI");
        //if (currentRole.GetEnemyTag() == TagMapping.Player.ToString())
        //{
        //    return playerObj;
        //}

        return playerObj;
    }

    public void OnMeeleAttackTrigger(string state)
    {
        if (state == "Trigger") {
           // Debug.Log("Triggerd attack: " + Time.time);
            GameObject attackSlash = (GameObject)Object.Instantiate(Resources.Load("Prafabs/Effect/AttackLightSlash"));
            // GameObject attackSlash = PrafabUtils.Instance.create("/Effect/AttackLightSlash");
            attackSlash.transform.position = shootPoint.position;
            attackSlash.transform.rotation = shootPoint.rotation;
            //attackSlash.GetComponent<ParticleSystem>().Play();

            if (aiUtils.CheckFanArea(gameObject.transform, GetTargetObject().transform, 45, currentRole.attackDistance)) {
                Role targetRole = GetTargetRole();
                float damage = currentRole.attribute.attack;
                targetRole.ReduceHealth(damage);
            }

        }
        if (state == "End") {
           // Debug.Log("attack End " + Time.time);
            progress = AIProgress.END;
        }
        
    }

    public void OnRangeAttackTrigger(string state)
    {
        if (state == "Trigger")
        {
            // Debug.Log("Triggerd attack: " + Time.time);
            aiUtils.Shoot(gameObject, shootPoint, GetTargetObject().transform, currentRole.GetBulletName());
        }
        if (state == "End")
        {
            // Debug.Log("attack End " + Time.time);
            progress = AIProgress.END;
        }
    }

    public void OnReactHitEnd() {
        progress = AIProgress.END;
    }

    public virtual void RegisterAnimationEvent()
    {
        eventHelper.notifyMeleeAttack += OnMeeleAttackTrigger;
        eventHelper.notifyRangeAttack += OnRangeAttackTrigger;
        eventHelper.notifyGetHitEnd += OnReactHitEnd;
    }

    // Start is called before the first frame update
    void Start()
    {
        RegisterAnimationEvent();
        updateFrequency = 0.5f;   
    }

  
}
