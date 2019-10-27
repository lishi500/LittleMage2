using ECM.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Role
{
    // Start is called before the first frame update
    PlayerMovementController playerMovementController;
    PlayerController playerController;
    public int maxSkillTriggerTimes = 1;

    public bool CanAttack()
    {
        return base.CanAction() && !playerMovementController._isMoving && !isChanneling;
    }

    public override void Awake()
    {
        base.Awake();
        playerMovementController = gameObject.GetComponentInParent<PlayerMovementController>();
        playerController = GetComponent<PlayerController>();

        alignment = AlignmentType.Player;

        GameObject gameManager = GameObject.FindGameObjectWithTag("GameController");
        PrafabHolder prafabHolder = gameManager.GetComponent<PrafabHolder>();

        GameObject MageFireBall = prafabHolder.GetSkill("MageFireBall");
        AddSkill(MageFireBall);

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
}
