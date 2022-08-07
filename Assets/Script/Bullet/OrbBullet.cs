using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class OrbBullet : Bullet
{
    [Header("Orb Skill")]

    [HideInInspector]
    public OrbSkill orbSkill;

    protected override void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("OrbBullet " + GetComponent<Rigidbody>().velocity.magnitude);

        GameObject hit = collision.gameObject;
        contactPoint = collision.contacts[0];

        if (creatorRole.GetEnemyTags().Contains(hit.tag))
        {
            TriggerSkill(hit);
            PlayOnHitEffect();

            EnemyHitProcess(hit);
           
        }
        else if (hit.tag == "Wall" || hit.layer == LayerMapping.Wall)
        {
            if (enhancement.isBounce && enhancement.bouncedCount < enhancement.maxBounceCount)
            {
                ProcessBounceBullet();
            }
            else
            {
                TriggerSkill(hit);
                PlayOnHitEffect();

                shouldDestory = true;
            }
           
        }
    }

    protected void EnemyHitProcess(GameObject hitTarget) {
        Enemy enemy = hitTarget.GetComponent<Enemy>();

        if (orbSkill.orbEnhancement == null)
        {
            enemy.ReduceHealth(Damage(enemy));

            hittedObjects.Add(hitTarget);
            Physics.IgnoreCollision(hitTarget.GetComponent<Collider>(), bulletCollider);
            shouldDestory = true;
        }
        else {
            ProcessEnhancement(hitTarget);
        }
       
    }
   

    public override void Start() {
        base.Start();
    }


    private void TriggerSkill(GameObject hit) {
        OrbController orbController = GetComponent<OrbController>();

        if (orbController != null) {
            orbController.TriggerSkill(hit, contactPoint);
        }
    }

    public void EnhancementMerge() {
        BulletEnhancement skillEnhance = orbSkill.orbEnhancement;
        BulletEnhancement bulletEnhance = enhancement;

        if (skillEnhance == null) {
            enhancement = bulletEnhance;
            return;
        }

        BulletEnhancement merged = ScriptableObject.CreateInstance<BulletEnhancement>();
        merged.isDamageEnhanced = skillEnhance.isDamageEnhanced || bulletEnhance.isDamageEnhanced;
        merged.damage = MergeValue(skillEnhance.damage, bulletEnhance.damage);
        merged.damageMultiplier = MergePercentage(skillEnhance.damageMultiplier, bulletEnhance.damageMultiplier);

        merged.damageType = bulletEnhance.damageType == DamageType.HOLY ? DamageType.HOLY : skillEnhance.damageType;

        merged.criticalAdder = MergeValue(skillEnhance.criticalAdder, bulletEnhance.criticalAdder);
        merged.speedMultiplier = MergePercentage(skillEnhance.speedMultiplier, bulletEnhance.speedMultiplier);
        merged.sizeMultiplier = MergePercentage(skillEnhance.sizeMultiplier, bulletEnhance.sizeMultiplier);
        merged.massMultiplier = MergePercentage(skillEnhance.massMultiplier, bulletEnhance.massMultiplier);

        // bullet multi
        merged.multiBulletNumber = MergeValue(skillEnhance.multiBulletNumber, bulletEnhance.multiBulletNumber); // extra bullet
        merged.multiDecayedPercentage =TriMin(skillEnhance.multiDecayedPercentage, bulletEnhance.multiDecayedPercentage, 0.2f);

        // bullet chain
        merged.dartleBulletNumber = MergeValue(skillEnhance.dartleBulletNumber, bulletEnhance.dartleBulletNumber); // extra bullet
        merged.dartleDecayedPercentage = TriMin(skillEnhance.dartleDecayedPercentage, bulletEnhance.dartleDecayedPercentage, 0.2f);

        // bullet split
        merged.splitBulletNumber = MergeValue(skillEnhance.splitBulletNumber, bulletEnhance.splitBulletNumber);
        merged.maxSplitTimes = MergeValue(skillEnhance.maxSplitTimes, bulletEnhance.maxSplitTimes) - 2; // default 2
        merged.splitDecayedPercentage = TriMin(skillEnhance.splitDecayedPercentage, bulletEnhance.splitDecayedPercentage, 0.4f); 
        merged.splitedTimeCount = Mathf.Max(skillEnhance.splitedTimeCount, bulletEnhance.splitedTimeCount);

        // bullet piercing
        merged.isPirercing = skillEnhance.isPirercing || bulletEnhance.isPirercing;
        merged.isPirercingDecayed = skillEnhance.isPirercingDecayed && bulletEnhance.isPirercingDecayed;
        merged.pirercingDecayedPercentage = TriMin(skillEnhance.pirercingDecayedPercentage, bulletEnhance.pirercingDecayedPercentage, 0.25f);
        merged.minPirercingDamagePercentage = TriMax(skillEnhance.minPirercingDamagePercentage, bulletEnhance.minPirercingDamagePercentage, 0.4f);
        merged.maxNumberOfPirercing = TriMin(skillEnhance.maxNumberOfPirercing, bulletEnhance.maxNumberOfPirercing, 1000);
        merged.pirercingTriggeredCount = Mathf.Max(skillEnhance.pirercingTriggeredCount, bulletEnhance.pirercingTriggeredCount);

        // bullet bounce 
        merged.isBounce = skillEnhance.isBounce || bulletEnhance.isBounce;
        merged.maxBounceCount = MergeValue(skillEnhance.maxBounceCount, bulletEnhance.maxBounceCount) - 3; // default 3
        merged.bounceDecayedPercentage = TriMin(skillEnhance.bounceDecayedPercentage, bulletEnhance.bounceDecayedPercentage, 0.3f);
        merged.bouncedCount = Mathf.Max(skillEnhance.bouncedCount, bulletEnhance.bouncedCount);

        // bullet explode
        merged.isExplode = skillEnhance.isExplode || bulletEnhance.isExplode;
        merged.explodeRadius = MergeValue(skillEnhance.explodeRadius, bulletEnhance.explodeRadius) - 5;
        merged.explodeFanAngle = Mathf.Max(skillEnhance.explodeFanAngle, bulletEnhance.explodeFanAngle);
        merged.explodeCenterDamage = TriMax(skillEnhance.explodeCenterDamage, bulletEnhance.explodeCenterDamage, 0.5f);
        merged.explodeEdgeDamage = TriMax(skillEnhance.explodeEdgeDamage, bulletEnhance.explodeEdgeDamage, 0.25f);
        merged.knockBackDistance = MergeValue(skillEnhance.knockBackDistance, bulletEnhance.knockBackDistance);

        // bullet return
        merged.isReturn = skillEnhance.isReturn || bulletEnhance.isReturn;
        merged.isReturnCaster = skillEnhance.isReturnCaster && bulletEnhance.isReturnCaster;
        merged.maxReturnDistance = MergeValue(skillEnhance.maxReturnDistance, bulletEnhance.maxReturnDistance) - 15; 
        merged.returnDecayedPercentage = TriMin(skillEnhance.returnDecayedPercentage, bulletEnhance.returnDecayedPercentage, 0.5f);
        merged.returnTriggered = skillEnhance.returnTriggered || bulletEnhance.returnTriggered;
        merged.returnComplete = skillEnhance.returnComplete || bulletEnhance.returnComplete;

        // bullet tracking
        merged.isTracking = skillEnhance.isTracking || bulletEnhance.isTracking;
        merged.trackingAngle = TriMax(skillEnhance.trackingAngle, bulletEnhance.trackingAngle, 30);
        merged.trackingIntensity = TriMax(skillEnhance.trackingIntensity, bulletEnhance.trackingIntensity, 5);

        enhancement = merged; 
    }

    private float MergeValue(float a, float b)
    {
        return a + b;
    }
    private int MergeValue(int a, int b)
    {
        return a + b;
    }
    private float MergePercentage(float a, float b)
    {
        if (a == 0)
        {
            return b;
        }
        else if (b == 0)
        {
            return a;
        }
        else
        {
            return a * b;
        }
    }
    private float TriMin(float a, float b, float c)
    {
        return Mathf.Min(Mathf.Min(a, b), c);
    }
    private float TriMax(float a, float b, float c)
    {
        return Mathf.Max(Mathf.Max(a, b), c);
    }
    private int TriMin(int a, int b, int c)
    {
        return Mathf.Min(Mathf.Min(a, b), c);
    }
    private int TriMax(int a, int b, int c)
    {
        return Mathf.Max(Mathf.Max(a, b), c);
    }
   
}
