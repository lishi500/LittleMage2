using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbSkill : Skill
{
    [Header("Orb Skill")]
    public BulletEnhancement orbEnhancement;
    public GameObject bulletPrafab;
    public GameObject bulletImpactPrafab;

    public GameObject staffEffectPrafab;

    public override void OnSkillAd()
    {
    }

    public override void OnSkillCast()
    {
    }

    public override void SkillSetup()
    {
    }

    public override void UpdateCollider()
    {
    }

    public override void UpdateEffect()
    {
    }

    public virtual void OnBulletMoving() {
    }

    public virtual bool OnBulletTrigger() {
        return true;
    }

    void Awake()
    {
        type = SkillType.ORB;
    }
}
