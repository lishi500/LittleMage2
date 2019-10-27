using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    ShootController shootController;
    PlayerAnimationController animationController;
    SimpleEventHelper eventHelper;
    PlayerController playerController;

    Player player;
    private int MAX_COUNT = 3;
    private int attackCount;
    Transform target;

    [HideInInspector]
    public float lastAttackTimer;
    [HideInInspector]
    public bool isAttacking;
    [HideInInspector]
    public PrafabHolder prafabHolder;

    private Vector3 gizmosShootPoint;
    private Vector3 gizmosReceivePoint;

    private GameObject nextObrBullet;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gizmosReceivePoint, 0.5f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(gizmosShootPoint, 0.5f);

    }

    public void Attack(Transform target)
    {
        //Debug.Log("AttackPosition ");
        isAttacking = true;
        this.target = target;
        SwitchToNextAttackAnimation();
        animationController.SetAttackCount(attackCount);
        animationController.OnAttackSpeedChange(player.attribute.attackSpeed);

        lastAttackTimer = Time.time;
    }

    public void FightStance() {
        animationController.animationState = AnimationState.FIGHT_STANCE;
        isAttacking = false;
    }

    public void OnAttackFired(string state)
    {
        //Debug.Log("Attack Fired: " + Time.time + "  - > " + state );
       
        if (state == "Trigger")
        {
            isAttacking = true;
            Transform shootpoint = GetNextAttackShootPoint();
            if (nextObrBullet != null)
            {
                //Debug.Log("Shoot Orb  !!!");
                shootController.ShootOrb(nextObrBullet, gameObject, shootpoint, target);

                // reset orb
                playerController.nextOrbSkill = null;
                nextObrBullet = null;
            }
            else
            {
                //Debug.Log("Shoot Bullet  !!!");
                shootController.Shoot(gameObject, shootpoint, target);
            }


            // gizmos
            gizmosShootPoint = shootpoint.position;
            gizmosReceivePoint = target.position;
        }
        else
        { // End
            AddCount();

            if (!CanAttack()) // && !player.isChanneling
            {
                FightStance();
            }
        }
    }

    public void CreateOrb(OrbSkill skill) {
        nextObrBullet = Instantiate(prafabHolder.OrbBullet, player.GetShootPoint(ShootPointPosition.MID).position, Quaternion.identity);
        OrbController orbController = nextObrBullet.GetComponent<OrbController>();
        orbController.skill = skill;
        orbController.caster = player;

        GameObject skillPrafab = prafabHolder.GetSkill(skill.skillName);
        OrbSkillController orbSkillControllerComponent = skill.GetComponent<OrbSkillController>();

        OrbSkillController orbSkillController = (OrbSkillController) nextObrBullet.AddComponent(orbSkillControllerComponent.GetType());
        orbSkillController.skill = skill;
        orbSkillController.creator = gameObject;

        orbController.orbSkillController = orbSkillController;
        //nextObrBullet.GetComponent<Bullet>().creator = gameObject;
    }

    



    private Transform GetNextAttackShootPoint() {
        return player.GetShootPoint(ShootPointPosition.MID);
        //switch (attackCount) {
        //    case 0:
        //        return player.GetShootPoint(ShootPointPosition.DEFAULT);
        //    case 1:
        //        return player.GetShootPoint(ShootPointPosition.DEFAULT);
        //    case 2:
        //        return player.GetShootPoint(ShootPointPosition.DEFAULT);
        //    //case 3:
        //    //    return player.GetShootPoint(ShootPointPosition.DEFAULT);
        //    default:
        //        return player.GetShootPoint(ShootPointPosition.DEFAULT);
        //}
    }

    public void SwitchToNextAttackAnimation() {
        switch (attackCount) {
            case 0:
                animationController.animationState = AnimationState.ATTACK1;
                break;
            case 1:
                animationController.animationState = AnimationState.ATTACK2;
                break;
            case 2:
                animationController.animationState = AnimationState.ATTACK3;
                break;
            default:
                animationController.animationState = AnimationState.ATTACK1;
                break;
        }
        
    }

    public float GetAttackRate()
    {
        return 10 / player.attribute.attackSpeed;
    }

    public void ResetCount() {
        attackCount = 0;
        isAttacking = false;
        animationController.SetAttackCount(-1);
    }

    public void AddCount() {
        attackCount++;
        if (attackCount >= MAX_COUNT) {

            ResetCount();
        }
    }

    public bool CanAttack()
    {
        if (player.CanAttack() && Time.time >= lastAttackTimer + GetAttackRate())
        {

            return true;
        }
        return false;
    }



    // Update is called once per frame
    void Awake()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameController");
        prafabHolder = gameManager.GetComponent<PrafabHolder>();

        player = GetComponent<Player>();
        shootController = GetComponent<ShootController>();
        playerController = GetComponent<PlayerController>();

        animationController = GetComponentInChildren<PlayerAnimationController>();
        eventHelper = GetComponentInChildren<SimpleEventHelper>();

        eventHelper.notifyRangeAttack += OnAttackFired;
    }
}
