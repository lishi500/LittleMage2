using ECM.Controllers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public Role currentRole;
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
    public GameObject playerObj;
    //[HideInInspector]
    //public GameObject targetObject;
    [HideInInspector]
    public Vector3 patrolPoint;
    [HideInInspector]
    protected float lastAttackTimmer;

    protected bool skillReady {
        get { return readySkills.Count > 0;  }
    }
    protected List<Skill> readySkills;
    protected Skill currentSkill;

    public abstract void SwitchState();
    public abstract void UpdateBehaviour();
    public abstract void InProgressMonitor();
    public abstract GameObject GetTargetObject();

    public virtual void OnSkillReady(Skill skill)
    {
        readySkills.Add(skill);
        //Debug.Log("Pet Skill Ready " + skill.name);
    }

    public virtual void Update()
    {
        if (currentRole.isAlive) {
            if (progress == AIProgress.END)
            {
                progress = AIProgress.INIT;

                SwitchState();
                stateStartTime = Time.time;
                UpdateNow();
            }
            if (ShouldUpdate())
            {
                UpdateBehaviour();
            }
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

    public virtual void CastSkill(Skill skill, Transform target = null)
    {
        GameObject skillObj = Instantiate(prafabHolder.Skillholder, currentRole.GetShootPoint(skill.shootPointPosition).transform);

        SkillController skillController = skillObj.GetComponent<SkillController>();
        skillController.creator = gameObject;
        //skillObj.transform.position = target.position;
        skill.skillControllerObj = skillObj;
        skillController.skill = skill;
        skillController.position1 = target == null ? transform : target;
        skillController.InitialSkill();

        //if (skill.isChannelSkill)
        //{
        //    StartChannelingSkill();
        //    skillController.notifySkillFinish += EndChannelingSkill;
        //}
    }

    public void resetPatrolPoint() {
        patrolPoint = gameObject.transform.position;
    }

    protected bool ShouldShowHitAnimation(float damage, Role role) {
        float maxHp = role.attribute.maxHP;
        float extraChange = damage / maxHp;
        return UnityEngine.Random.Range(0, 1f) + extraChange > role.attribute.toughness;
    }

    private SkillType[] ALL_SKILL_TYPES = System.Enum.GetValues(typeof(SkillType)).Cast<SkillType>().ToArray();
    protected Skill GetNextReadySkill(SkillType[] skillTypes = null) {
        if (skillTypes == null || skillTypes.Length == 0) {
            skillTypes = ALL_SKILL_TYPES;
        }

        foreach (Skill skill in readySkills) {
            if (skillTypes.Contains(skill.type)) {
                return skill;
            }
        }

        return null;
    }

    public virtual void Start() { }
    public virtual void Awake()
    {
        baseAgentController = transform.GetComponent<BaseAgentController>();
        GameObject gameManager = Finder.Instance.GetGameManager().gameObject;
        prafabHolder = Finder.Instance.GetPrafabHolder();
        aiUtils = gameManager.GetComponentInChildren<AIUtils>();
        animationController = transform.GetComponentInChildren<CustomAnimationController>();
        eventHelper = transform.GetComponentInChildren<SimpleEventHelper>();
        shootPoint = aiUtils.GetShootPoint(transform);

        playerObj = Finder.Instance.GetPlayer().gameObject;
        currentRole = GetComponent<Role>();
        currentRole.statusManager.notifyStatusChange += OnStatusChange;
        eventHelper.notifyGetHit += OnGetHit;
        eventHelper.notifySkillReady += OnSkillReady;

        readySkills = new List<Skill>();

        currentState = AIState.IDLE;
        stateStartTime = Time.time;
        progress = AIProgress.END;
        resetPatrolPoint();
    }

    // ----------------- Action

    public virtual void PatrolAction()
    {
        animationController.animationState = AnimationState.WALK;

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

        if (arrived)
        {
            progress = AIProgress.END;
        }
    }

    public virtual void ApproachingAction()
    {
        animationController.animationState = AnimationState.RUN;

        bool arrived = false;
        if (progress == AIProgress.INIT)
        {
            progress = AIProgress.RUNNING;
        }

        if (progress == AIProgress.RUNNING)
        {
            arrived = ChasingTarget(GetTargetObject().transform, currentRole.attackDistance);
            //Debug.Log("ApproachingAction " + currentRole .name + " -> "+ GetTargetObject().name + "  = " + arrived);
        }

        if (arrived)
        {
            progress = AIProgress.END;
        }
    }

    public virtual void AttackAction()
    {
        animationController.animationState = AnimationState.ATTACK;

        if (progress == AIProgress.INIT || progress == AIProgress.RUNNING)
        {
            progress = AIProgress.RUNNING;
            FaceAtTarget(GetTargetObject().transform);
        }
    }

    public virtual void ReactHitAction()
    {
        if (progress == AIProgress.INIT)
        {
            Debug.Log("SimpleAI ReactHitAction ");
            progress = AIProgress.RUNNING;
            animationController.RestartGetHit();
        }
    }
}
