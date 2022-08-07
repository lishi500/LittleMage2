using System.Linq;
using ECM.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    public float attackAnimationLength;

    public float publicCD;
    //[HideInInspector]
    //public float publicCDPast;
    [HideInInspector]
    public Player player;
    [HideInInspector]
    public PrafabHolder prafabHolder;
    [HideInInspector]
    public SimpleEventHelper eventHelper;

    public Transform shootPointHand;
    public Transform shootPointStaff;
    public Transform shootPointMid;
    public Transform receivePoint;

    PlayerAnimationController animationController;
    PlayerMovementController playerMovementController;
    AttackController attackController;
    ShootController shootController;
    AutoAming autoAming;

    [HideInInspector]
    public OrbSkill nextOrbSkill;

    private SkillType[] orbSkillType = new SkillType[] { SkillType.ORB };

    public Transform target {
        get {
            return autoAming.currentTarget == null ? null : autoAming.currentTarget.transform;
        }
    }

    public void NextPlayerAction() {
        if (!IsMoving())
        {
            if (target != null) {
                LookToTarget();

                if (attackController.CanAttack())
                {
                    attackController.Attack(target);
                }
            }
        }

        if (nextOrbSkill == null) {
            OrbSkill OrbSkill = GetNextReadyOrbSkill();
            if (OrbSkill != null) {
                //Debug.Log("next ready orb skill " + OrbSkill.name);
                nextOrbSkill = OrbSkill;
                attackController.CreateOrb(nextOrbSkill);
            }
        }
    }

    public bool IsMoving() {
        return playerMovementController._isMoving;
    }

    public void AttackPosition(Transform target)
    {
        if (attackController.CanAttack())
        {
            attackController.Attack(target);
        }
    }

    public void CastSkill(Skill skill) {
        CastSkill(skill, target);
    }

    public void CastSkill(Skill skill, Transform target)
    {
        GameObject skillObj = Instantiate(prafabHolder.Skillholder, player.GetShootPoint(skill.shootPointPosition).transform);

        SkillController skillController = skillObj.GetComponent<SkillController>();
        skillController.creator = gameObject;
        //skillObj.transform.position = target.position;
        skill.skillControllerObj = skillObj;
        skillController.skill = skill;
        skillController.position1 = target;
        skillController.InitialSkill();
        if (skill.isChannelSkill) {
            StartChannelingSkill();
            skillController.notifySkillFinish += EndChannelingSkill;
        }

    }

    public OrbSkill GetNextReadyOrbSkill() {
        Skill skill = player.GetMinCDSkill(orbSkillType);
        if (skill != null && skill.IsReady()) {
            //Debug.Log("GetNextReadyOrbSkill ready: " + skill.skillName);
            return (OrbSkill) skill;
        }

        return null;
    }
   
    private void UpdateAnimationStatus() {
        if (playerMovementController._isMoving)
        {
            animationController.animationState = AnimationState.RUN;
            attackController.ResetCount();
        }
        else if (!attackController.isAttacking && target != null)
        {
            animationController.animationState = AnimationState.FIGHT_STANCE;
        }
        else {
            animationController.animationState = AnimationState.IDLE;
        }
        //else if (!isAttacking && gameObject.GetComponent<AutoAming>().GetNearestAttackableEnemy() != null)
        //{
        //    //animationController.AddException("Run", false);
        //    animationController.animationState = AnimationState.ATTACK;
        //}
        //else if (!attackController.isAttacking) {
        //    animationController.animationState = AnimationState.IDLE;
        //}
    }

    [Obsolete]
    public bool CanCastSkill()
    {
        //return publicCDPast <= 0;
        return true;
    }
    public void StartChannelingSkill() {
        //Debug.Log("StartChannelingSkill " + Time.time);

        //player.isChanneling = true;
        //animationController.AddException("channeling", true);
        //animationController.animationState = AnimationState.FIGHT_STANCE;
    }
    public void EndChannelingSkill() {
        //Debug.Log("EndChannelingSkill " + Time.time);
        //player.isChanneling = false;
        //animationController.ClearException();
        //animationController.AddExceptionOnce("channeling", false);
        //animationController.AddExceptionOnce("channeling_loop", false);
    }

    public void PrepareNextOrbSkill() {
        if (nextOrbSkill != null) {
            attackController.CreateOrb(nextOrbSkill);
        }
    }
    public void OnSkillReady(Skill skill) {
        if (skill.type == SkillType.ORB && nextOrbSkill == null) {
            nextOrbSkill = (OrbSkill)skill;

        }
    }
  

    void RegisterAttributeListener() {
        if (animationController != null) {
            player.attribute.notifyAttackSpeedChange += animationController.OnAttackSpeedChange;
        }
    }

    void RegisterSkillListener() {
        if (eventHelper != null) {
            eventHelper.notifySkillReady += OnSkillReady;
        }
    }

    // Fix Call
    void Awake()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameController");
        prafabHolder = gameManager.GetComponent<PrafabHolder>();

        playerMovementController = gameObject.GetComponentInParent<PlayerMovementController>();
        animationController = GetComponentInChildren<PlayerAnimationController>();
        eventHelper = GetComponentInChildren<SimpleEventHelper>();


        player = GetComponent<Player>();
        shootController = GetComponent<ShootController>();
        attackController = GetComponent<AttackController>();
        autoAming = GetComponent<AutoAming>();

    }

    void Update()
    {
        UpdateAnimationStatus();
        NextPlayerAction();
    }

    private void LateUpdate()
    {
    }

    private void LookToTarget()
    {
        if (target != null) {
            Vector3 targetDir = target.transform.position - transform.position;
            playerMovementController.RotateTowards(targetDir);
        }
    }
}
