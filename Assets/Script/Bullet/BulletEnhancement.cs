using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "Bullet/bullet enhancement", order = 1)]
public class BulletEnhancement : ScriptableObject
{
    [Header("Bullet Basic Enhancement")]
    public bool isDamageEnhanced = false;
    public float damage;
    public float damageMultiplier = 1;

    public DamageType damageType = DamageType.NORMAL;

    public float criticalAdder = 0;
    public float speedMultiplier = 1;
    public float sizeMultiplier = 1;
    public float massMultiplier = 1;

    [Header("Bullet Multi")]
    // bullet multi
    public int multiBulletNumber = 0; // extra bullet
    public float multiDecayedPercentage = 0.15f;

    [Header("Bullet Dartle")]
    // bullet chain
    public int dartleBulletNumber = 0;
    public float dartleDecayedPercentage = 0.1f;
    public float dartleMinPercentage = 0.4f;

    [Header("Bullet Split")]
    // bullet split
    public int splitBulletNumber = 0;
    public int maxSplitTimes = 2;
    public float splitDecayedPercentage = 0.4f;
    public int splitedTimeCount = 0;

    [Header("Bullet Piercing")]
    // bullet piercing
    public bool isPirercing = false;
    public bool isPirercingDecayed = true;
    public float pirercingDecayedPercentage = 0.25f;
    public float minPirercingDamagePercentage = 0.4f;
    public int maxNumberOfPirercing = 1000;
    public int pirercingTriggeredCount = 0;

    // bullet bounce 
    [Header("Bullet Bounce")]
    public bool isBounce = false;
    public int maxBounceCount = 3;
    public float bounceDecayedPercentage = 0.3f;
    public int bouncedCount = 0;

    // bullet explode
    [Header("Bullet Explode")]
    public bool isExplode = false;
    public float explodeRadius = 5f;
    public float explodeFanAngle = 0f;
    public float explodeCenterDamage = 0.5f; // 33% of radius is inside center 
    public float explodeEdgeDamage = 0.25f;
    public float knockBackDistance = 0f;
    public GameObject explodeEffect;

    // bullet return
    [Header("Bullet Return")]
    public bool isReturn = false;
    public bool isReturnCaster = true; // return to owner
    public float maxReturnDistance = 15f;
    public float returnDecayedPercentage = 0.5f;
    public bool returnTriggered = false;
    public bool returnComplete = false;

    // bullet tracking
    [Header("Bullet Tracking")]
    public bool isTracking = false;
    public float trackingRange = 15f;
    public int trackingAngle = 90;
    public float trackingIntensity = 5f;
}
