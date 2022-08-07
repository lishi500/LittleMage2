using ECM.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : Role
{
    // Start is called before the first frame update
    PlayerMovementController playerMovementController;
    PlayerController playerController;
    [Header("------------- Player Parameters --------------")]
    public int maxSkillTriggerTimes = 1;
    public UISkillController uISkillController;
    public LevelExpData levelExpData;

    public Transform target {
        get { return playerController.target; } 
    }

    public delegate void ExpGainDelegate(int exp);
    public event ExpGainDelegate notifyExpGain;

    public delegate void LevelUpDelegate(int level);
    public event LevelUpDelegate notifyLevelUp;

    public bool CanAttack()
    {
        return base.CanAction() && !playerMovementController._isMoving && !isChanneling;
    }

    protected override void OnAddSkill(Skill skill)
    {
        uISkillController.FlushSpellSlot();
    }

    public Skill[] GetTopOrbSkillsOrderByCDLedt(int k) {
        if (skills != null && skills.Count > 0)
        {
            Skill[] minCdSkill = skills.Where(skill => skill.type == SkillType.ORB)
                .OrderBy(vi => vi.CDLeft)
                .ThenBy(vi => vi.skillName)
                .Take(k)
                .ToArray();

            return minCdSkill;
        }

        return null;
    }

    public override void Awake()
    {
        base.Awake();
        playerMovementController = gameObject.GetComponentInParent<PlayerMovementController>();
        playerController = GetComponent<PlayerController>();

        roleType = RoleType.Player;

        uISkillController = UIFinder.Instance.GetUISkillController();

        //GameObject MageFireBall = prafabHolder.GetSkill("Mega Fire Ball");
        //AddSkill(MageFireBall);

        //GameObject AttackUp = prafabHolder.GetSkill("AttackUp");
        //AddSkill(AttackUp);

        //GameObject IceSpikeSkill = prafabHolder.GetSkill("IceSpike");
        //AddSkill(IceSpikeSkill);

        //GameObject BlizardSkill = prafabHolder.GetSkill("Blizard");
        //AddSkill(BlizardSkill);

        //GameObject FireSpraySkill = prafabHolder.GetSkill("FireSpray");
        //AddSkill(FireSpraySkill);

        //GameObject LightningStrick = prafabHolder.GetSkill("LightningStrick");
        //AddSkill(LightningStrick);

        //GameObject AttackSpeedUp = prafabHolder.GetSkill("AttackSpeedUp");
        //AddSkill(AttackSpeedUp);

        //GameObject AttackSpeedBuff1 = Instantiate(prafabHolder.GetBuff("attackSpeed"));
        //AddBuff(AttackSpeedBuff1, GetComponent<Role>());
        //GameObject AttackSpeedBuff2 = Instantiate(prafabHolder.GetBuff("attackSpeed"));
        //AddBuff(AttackSpeedBuff2, GetComponent<Role>());
    }

    public override Transform GetShootPoint(ShootPointPosition pointType)
    {
        switch (pointType)
        {
            case ShootPointPosition.HAND:
                return playerController.shootPointHand;
            case ShootPointPosition.STAFF:
                return playerController.shootPointStaff;
            case ShootPointPosition.MID:
                return playerController.shootPointMid;
            case ShootPointPosition.SELF:
                return gameObject.transform;
            case ShootPointPosition.RECEIVE:
                return playerController.receivePoint;
            default:
                return playerController.shootPointMid;
        }
    }

    public void GainExperience(int exp) {
        attribute.exp += exp;
        if (notifyExpGain != null) {
            notifyExpGain(exp);
        }

        if (levelExpData.CanLevelUp(attribute.level, attribute.exp)) {
            attribute.level += 1;
            attribute.exp -= levelExpData.expMap[attribute.level];

            if (notifyLevelUp != null) {
                notifyLevelUp(attribute.level);
            }
        }
    }

    public void LootSkills(int numberOfSkills) {


    }
}
