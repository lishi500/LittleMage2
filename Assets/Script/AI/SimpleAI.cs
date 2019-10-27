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
                animationController.animationState = AnimationState.WALK;
                PartrolAction();
                break;
            case AIState.APPROACHING:
                animationController.animationState = AnimationState.RUN;
                ApproachingAction();
                break;
            case AIState.ATTACK:
                animationController.animationState = AnimationState.ATTACK;
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

    void PartrolAction() {
        bool arrived = false;
        if (progress == AIProgress.INIT)
        {
            patrolPoint = aiUtils.GenerateRandomPosition(gameObject.transform);
            arrived = MoveToDestination(patrolPoint);
            progress = AIProgress.RUNNING;
        }
        else if (progress == AIProgress.RUNNING)
        {
            arrived = MoveToDestination(patrolPoint);
        }

        if (arrived) {
            //Debug.Log("End of PartrolAction ");
            progress = AIProgress.END;
        }
    }

    void ApproachingAction() {
        bool arrived = false;
        if (progress == AIProgress.INIT)
        {
            progress = AIProgress.RUNNING;
        }

        if (progress == AIProgress.RUNNING)
        {
            arrived = ChasingTarget(GetTargetObject().transform, currentRole.attackDistance);
        }

        if (arrived)
        {
          //  Debug.Log("End of ApproachingAction ");
            progress = AIProgress.END;
        }
    }

    void AttackAction() {
        //Debug.Log("AttackAction" + progress);

        if (progress == AIProgress.INIT || progress == AIProgress.RUNNING)
        {
            progress = AIProgress.RUNNING;
            FaceAtTarget(GetTargetObject().transform);
        }
    }

    void ReactHitAction() {
        if (progress == AIProgress.INIT)
        {
            Debug.Log("SimpleAI ReactHitAction ");
            progress = AIProgress.RUNNING;
            animationController.RestartGetHit();
        }
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
            aiUtils.Shoot(gameObject, shootPoint, GetTargetObject().transform, currentRole.bulletName);
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
