using ECM.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseAI : MonoBehaviour
{
    // Start is called before the first frame update
    GameManager gameManager;
    [HideInInspector]
    public BaseAgentController baseAgentController;
    [HideInInspector]
    public PrafabHolder prafabHolder;
    [HideInInspector]
    public Enemy currentRole;
    [HideInInspector]
    public AIState currentState;
    [HideInInspector]
    public float stateStartTime;
    [HideInInspector]
    public AIProgress progress;
    [HideInInspector]
    public AIUtils aiUtils;
    [HideInInspector]
    public CustomAnimationController animationController;
    [HideInInspector]
    public SimpleEventHelper eventHelper;
    [HideInInspector]
    public Transform shootPoint;

    public float updateFrequency = 2f; // update behaviour every 2s (smartness)
    public float agreesive = 0.1f; // 0.1 - 1
    public float baseAgreesivePercentage = 0.1f;
    private float waitTime = 0f;

    // AI run time object
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public GameObject targetObject;
    [HideInInspector]
    public Vector3 patrolPoint;

    public abstract void SwitchState();
    public abstract void UpdateBehaviour();
    public abstract void InProgressMonitor();

    public virtual void Update()
    {
        if (progress == AIProgress.END)
        {
            progress = AIProgress.INIT;

            SwitchState();
            stateStartTime = Time.time;
            UpdateNow();
        }
        if (ShouldUpdate()) {
            UpdateBehaviour();
        }
       
    }
    public void LateUpdate()
    {
        InProgressMonitor();
    }

    public bool ShouldUpdate() {
        waitTime += Time.deltaTime;
        if (waitTime >= updateFrequency) {
            waitTime = 0;
            return true;
        }
        return false;
    }
    public void UpdateNow() {
        waitTime = updateFrequency;
    }

    public virtual void OnGetHit(DamageDef damageDef)
    {
        if (ShouldShowHitAnimation(damageDef.damage, currentRole)) {
            currentState = AIState.REACT_HIT;
            progress = AIProgress.INIT;
        }
    }


    public bool IsNextActionAggressive(float modifier = 0f) {
        return baseAgreesivePercentage + agreesive + modifier >= Random.Range(0, 1f);
    }
    public virtual GameObject GetTargetObject()
    {
        return player;
    }
    public bool ChasingTarget(Transform target, float stopDistance)
    {
        if (currentRole.CanMove()) {
            return aiUtils.ChasingTarget(transform, target, stopDistance);
        }
        return false;
    }
    public bool MoveToDestination(Vector3 position, float stopDistance = 1f)
    {
        if (currentRole.CanMove())
        {
            return aiUtils.MoveToDestination(transform, position, stopDistance);
        }
        return false;
    }

    public void FaceAtTarget(Transform to)
    {
        if (currentRole.CanAction()) {
            aiUtils.FaceAtTarget(transform, to);
        }
    }

    private void OnStatusChange(RoleStatus roleStatus, bool isAdd) {
        NavMeshAgent agent = aiUtils.GetNavMeshAgent(gameObject.transform);

        if (agent.isStopped) {
            if (currentRole.CanMove()) {
                agent.isStopped = false;
            }
        } else {
            if (!currentRole.CanMove())
            {
                agent.isStopped = true;
            }
        }
    }
    public Role GetTargetRole() {
        GameObject target = GetTargetObject();
        Role role = null;
        role = target.GetComponent<Role>();
        if (role == null) {
            role = target.GetComponentInChildren<Role>();
        }
        if (role == null) {
            role = target.GetComponentInParent<Role>();
        }
        return role;
    }

    public void resetPatrolPoint() {
        patrolPoint = gameObject.transform.position;
    }

    protected bool ShouldShowHitAnimation(float damage, Role role) {
        float maxHp = role.attribute.maxHP;
        float extraChange = damage / maxHp;
        return Random.Range(0, 1f) + extraChange > role.attribute.toughness;
    }
  
    public virtual void Awake()
    {
        baseAgentController = transform.GetComponent<BaseAgentController>();
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameController");
        prafabHolder = gameManager.GetComponent<PrafabHolder>();
        aiUtils = gameManager.GetComponentInChildren<AIUtils>();
        animationController = transform.GetComponentInChildren<CustomAnimationController>();
        eventHelper = transform.GetComponentInChildren<SimpleEventHelper>();
        shootPoint = aiUtils.GetShootPoint(transform);

        player = GameObject.FindGameObjectWithTag("Player");
        currentRole = GetComponent<Enemy>();
        currentRole.statusManager.notifyStatusChange += OnStatusChange;
        eventHelper.notifyGetHit += OnGetHit;

        currentState = AIState.IDLE;
        stateStartTime = Time.time;
        progress = AIProgress.END;
        resetPatrolPoint();
    }
}
