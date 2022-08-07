using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleePetAI : PetAI
{
    public override void SwitchState()
    {
        //Debug.Log("SwitchState from [" + currentState + "]");

        switch (currentState) {
            case AIState.IDLE:
            case AIState.PATROL:
            case AIState.PLAY:
            case AIState.FOLLOW:
                SwitchFromSafeState();
                //currentState = AIState.PLAY;
                break;
            case AIState.APPROACHING:
                if(CanAttack())
                {
                    //Debug.Log("APPROACHING -> Can attack");
                    currentState = AIState.ATTACK;
                } else {
                    //Debug.Log("APPROACHING -> Cannot attack");
                    currentState = AIState.IDLE;
                }
                break;
            case AIState.ATTACK:
                currentState = AIState.APPROACHING;
                break;
            case AIState.CAST:
            case AIState.CREATE_ORB:
            case AIState.GUARD:
                break;

        }
    }


    

    public override void UpdateBehaviour()
    {
        if (isSafeState) {
            if (GetTargetObject() != null) {
                isSafeState = false;
                progress = AIProgress.END;

                return;
            }
        }

        if (CanCast()) {
            if (orbSkillReady)
            {
                progress = AIProgress.INIT;
                currentState = AIState.CREATE_ORB;
            }
            else if (skillReady)
            {
                Skill skill = GetNextReadySkill(ACTIVE_SKILL_TYPES);

                if (ShouldCast(skill))
                {
                    //Debug.Log("ShouldCast " + skill.name);
                    currentSkill = skill;
                    progress = AIProgress.INIT;
                    currentState = AIState.CAST;
                }
               
            }
        }
        

        switch (currentState) {
            case AIState.IDLE:
                animationController.animationState = AnimationState.IDLE;
                progress = AIProgress.INIT;
                break;
            case AIState.PATROL:
                PatrolAction();
                break;
            case AIState.PLAY:
                PlayAction();
                break;
            case AIState.FOLLOW:
                FollowAction();
                break;
            case AIState.APPROACHING:
                ApproachingAction();
                break;
            case AIState.ATTACK:
                AttackAction();
                break;
            case AIState.CAST:
                CastSkillAction();
                break;
        }
    }

    public override void InProgressMonitor()
    {
        switch (currentState)
        {
            case AIState.APPROACHING:
                if (aiUtils.HasReachDestination(aiUtils.GetNavMeshAgent(gameObject.transform)))
                {
                    progress = AIProgress.END;
                }
                break;
            default:
                break;
        }
    }

    public override void AttackAction()
    {
        if (progress == AIProgress.INIT) {
            animationController.AddAutoClearState(AnimationState.ATTACK, true);
            progress = AIProgress.RUNNING;
        }

        if (progress == AIProgress.RUNNING)
        {
            FaceAtTarget(GetTargetObject().transform);
        }
    }

    public void OnMeeleAttackTrigger(string state)
    {
        if (state == "Trigger")
        {
            // Debug.Log("Triggerd attack: " + Time.time);
            GameObject attackSlash = (GameObject)Object.Instantiate(Resources.Load("Prafabs/Effect/AttackLightSlash"));
            // GameObject attackSlash = PrafabUtils.Instance.create("/Effect/AttackLightSlash");

            attackSlash.transform.position = shootPoint.position;
            attackSlash.transform.rotation = shootPoint.rotation;
            //attackSlash.GetComponent<ParticleSystem>().Play();
            List<Transform> affectedEnemies = AttackUtils.Instance.FanAreaCollsion(pet.GetShootPoint(ShootPointPosition.MID), 90, pet.attackDistance, EnemyTags);

            foreach (Transform enemyTransform in affectedEnemies) {
                Enemy enemy = enemyTransform.GetComponent<Enemy>();
                float damage = currentRole.attribute.attack;
                enemy.ReduceHealth(damage);
            }
        }
        if (state == "End")
        {
            progress = AIProgress.END;
        }

    }

    public override void Start() {
        base.Start();
        eventHelper.notifyMeleeAttack += OnMeeleAttackTrigger;
    }
}
