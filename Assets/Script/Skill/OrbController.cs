using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbController : MonoBehaviour
{
    OrbStatus status;

    [HideInInspector]
    public OrbSkill skill;
    [HideInInspector]
    OrbBullet orbBullet;
    [HideInInspector]
    public Role caster;


    private PrafabHolder prafabHolder;
    private GameObject triggeredObject;
    private bool showStaffEffect = true;
    public OrbSkillController orbSkillController;
    private ContactPoint contactPoint;

    GameObject staffEffectObj;

    void OrbSkillReady() {
        status = OrbStatus.READY;
        if (showStaffEffect && skill != null && skill.staffEffectPrafab != null) {
            staffEffectObj = Instantiate(skill.staffEffectPrafab, caster.GetShootPoint(ShootPointPosition.STAFF));
        }
    }

    //void OrbAttackPreStart()
    //{
    //    status = OrbStatus.PRE_START;
    //}

    public void OrbAttackStart() {
        status = OrbStatus.MOVING;
        DestoryHandEffect();
        skill.StartCD();
    }

    public void DestoryHandEffect() {
        Destroy(staffEffectObj);
        showStaffEffect = false;
    }

    void OnOrbMoving() {
        if (orbSkillController != null) {
            orbSkillController.SkillOnTrail();
        }
    }

    public void TriggerSkill(GameObject triggeredObject, ContactPoint contactPoint) {
        status = OrbStatus.TRIGGERED;
        this.triggeredObject = triggeredObject;
        this.contactPoint = contactPoint;

        GameObject skillHolder = AddSkillController(triggeredObject, contactPoint);
        StartOrbSkill(skillHolder, triggeredObject);
    }

    private void StartOrbSkill(GameObject skillHolder, GameObject triggeredObject)
    {
        OrbSkillController skillController = skillHolder.GetComponent<OrbSkillController>();

        skillController.InitialSkill();
    }

    private GameObject AddSkillController(GameObject triggeredObject, ContactPoint contactPoint)
    {
        GameObject skillPrafab = prafabHolder.GetSkill(skill.skillName);
        OrbSkillController skillControllerPrafab = skill.GetComponent<OrbSkillController>();
        GameObject skillHolder = Instantiate(prafabHolder.Skillholder, transform.position, Quaternion.identity); ;

        OrbSkillController skillController = (OrbSkillController) skillHolder.AddComponent(skillControllerPrafab.GetType());
        skillController.primaryTarget = triggeredObject;
        skillController.creator = caster.gameObject;
        skillController.skill = skill;
        skillController.primaryContactPoint = contactPoint;
        skillController.primaryTarget = triggeredObject;
        skillController.position1 = transform;


        orbSkillController = skillController;
        skill.skillControllerObj = skillHolder;

        return skillHolder;

    }

    //void OrbPostTrigger() {
    //    status = OrbStatus.POST_TRIGGER;

    //}

    private void OnDestroy()
    {
        DestoryHandEffect();
    }


    void Update() {
        switch (status) {
            //case OrbStatus.PRE_READY:

            case OrbStatus.MOVING:
                OnOrbMoving();
                break;
        }
    }

    void Start() {
        orbBullet = GetComponent<OrbBullet>();
        OrbSkillReady();

        GameObject gameManager = GameObject.FindGameObjectWithTag("GameController");
        prafabHolder = gameManager.GetComponent<PrafabHolder>();
    }
}
