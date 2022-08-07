using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using System.Linq;

public class Bullet : MonoBehaviour
{

    public string bulletName;
    public BulletSize size = BulletSize.Small;
    public float bulletSpeed;
    public float autoDestroyTime;
    public int bulletId;
    public BulletEnhancement enhancement;

    [HideInInspector]
    public GameObject projectilePrafab;
    [HideInInspector]
    public GameObject impectPrafab;
    [HideInInspector]
    GameObject impectEffect;

    public bool usePhysicsToTranslate = true;
    [HideInInspector]
    public Vector3 direction;
    [HideInInspector]
    public List<GameObject> hittedObjects;
    //public GameObject OnHitEffect;
    [HideInInspector]
    public GameObject creator;
    [HideInInspector]
    public Role creatorRole;
    [HideInInspector]
    public GameObject aimingTarget;
    [HideInInspector]
    public float finalDamage;
    [HideInInspector]
    public float damageModifier = 1;
    [HideInInspector]
    public ContactPoint contactPoint;

    [HideInInspector]
    public bool shouldDestory = false;
    private bool shouldDestoryImpect = false;
    private Rigidbody bulletRigidbody;
    protected Collider bulletCollider;
    private Vector3 originalPoint;
    private float returnMagnitude;
    protected DamageUtils damageUtils;
    protected SmartMissile3D smartMissile;

    protected virtual void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Normal bullet " + GetComponent<Rigidbody>().velocity.magnitude);

        GameObject hit = collision.gameObject;
        contactPoint = collision.contacts[0];
        if (creatorRole.GetEnemyTags().Contains(hit.tag))
        {
            ApplyFinalDamage(hit);

            PlayOnHitEffect();
            shouldDestory = true;
        }
        else if (hit.tag == "Wall" || hit.layer == LayerMapping.Wall) {
            if (enhancement.isBounce && enhancement.bouncedCount < enhancement.maxBounceCount) {
                ProcessBounceBullet();
            } else {
                PlayOnHitEffect();
                shouldDestory = true;
            }
        }
    }


    protected virtual void PlayOnHitEffect() {
        if (impectPrafab != null) {
            impectEffect = GameObject.Instantiate(impectPrafab, contactPoint.point, Quaternion.identity);
            impectEffect.AddComponent<AutoDestroy>();
            ParticleSystem ps = impectEffect.GetComponent<ParticleSystem>();
            if (ps != null) {
                ps.Play();
            }
            shouldDestoryImpect = true;
        }
    }

    public void ApplyForce() {
        ApplyForce(direction, bulletSpeed);
    }

    public void ApplyForce(Vector3 direction, float speed) {
        if (usePhysicsToTranslate)
        {
            GetComponent<Rigidbody>().AddForce(-direction.normalized * speed * 1000);
            //Debug.DrawRay(transform.position, -direction, Color.red, 2f);

            //Debug.Log("Add force " + GetComponent<Rigidbody>().velocity.magnitude);
            //Debug.Log("Add force " + direction.normalized + "  " + direction);
        }
    }

    public void PreEnhancementProcess() {
        originalPoint = gameObject.transform.position;

        InitialFinalDamage();
        ProcessEnhanceDamage();
        ProcessPhysicalEnhancement();
        AddTracking();
    }

    public void InitialFinalDamage() {
        creatorRole = creator.GetComponent<Role>();
        finalDamage = creatorRole.attribute.attack;
    }

    public void ResetCollierIgnore() {
        //Physics.GetI
        foreach (GameObject hitted in hittedObjects) {
            if (hitted != null) {
                Physics.IgnoreCollision(bulletCollider, hitted.GetComponent<Collider>(), false);

            }
        }
        RestarTracking();
        hittedObjects.Clear();
    }

    public virtual void ProcessEnhancement(GameObject hitObject) {
        //ProcessSplitBullet();
        ProcessPricingBullet(hitObject);

        ApplyFinalDamage(hitObject);
        PlayOnHitEffect();

       // StartCoroutine(ProcessExplodeDamage());
        ProcessExplodeDamage();

        if (!enhancement.isPirercing) {
            shouldDestory = true;
        }
    }

    public void ApplyFinalDamage(GameObject hitObject)
    {
        Role hitRole = hitObject.GetComponent<Role>();
        if (hitRole != null && creatorRole.GetEnemyTags().Contains(hitObject.tag)) {
            hitRole.ReduceHealth(Damage(hitRole));
            hittedObjects.Add(hitObject);
            StopTracking();
        }
    }

    public virtual void ProcessEnhanceDamage() {
        if (enhancement.isDamageEnhanced)
        {
            //finalDamage = finalDamage * enhancement.damageMultiplier + enhancement.damage;
            finalDamage = finalDamage + enhancement.damage;
            damageModifier = damageModifier * enhancement.damageMultiplier;
        }
    }


    public virtual void ProcessSplitBullet()
    {

    }

    public virtual void ProcessPricingBullet(GameObject hitObject)
    {
        if (enhancement.isPirercing)
        {
            if (enhancement.pirercingTriggeredCount >= enhancement.maxNumberOfPirercing)
            {
                shouldDestory = true;
            }
            else
            {
                Physics.IgnoreCollision(hitObject.GetComponent<Collider>(), bulletCollider);
                shouldDestory = false;
                if (enhancement.isPirercingDecayed)
                {
                    float damagePercentageWithDecayed = 1 - enhancement.pirercingTriggeredCount * enhancement.pirercingDecayedPercentage;
                    damagePercentageWithDecayed = Mathf.Max(damagePercentageWithDecayed, enhancement.minPirercingDamagePercentage);
                    //finalDamage = finalDamage * damagePercentageWithDecayed;
                    damageModifier = damageModifier * damagePercentageWithDecayed;
                }
                ApplyForce(direction, bulletSpeed / 2);
                enhancement.pirercingTriggeredCount++;
            }
        }
    }

    public virtual void ProcessBounceBullet()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        Vector3 vel = rigidbody.velocity;
        Vector2 horizontalvector = new Vector2(vel.x, vel.z);
        horizontalvector = horizontalvector.normalized;
        float nextMagnitude = vel.magnitude < 25.0f ? 25f : vel.magnitude;
        horizontalvector = nextMagnitude * horizontalvector;
        rigidbody.velocity = new Vector3(horizontalvector.x, 0, horizontalvector.y);

        enhancement.bouncedCount++;
        ResetCollierIgnore();
    }

    public virtual void ProcessExplodeDamage()
    {
        if (enhancement.isExplode) {

            //yield return new WaitForSeconds(0.15f);

            string[] enemyTags = creator.GetComponent<Role>().GetEnemyTags();

            GameObject[] enemies = CommonUtils.Instance.GetObjectWithInRadius(contactPoint.point, enhancement.explodeRadius, enemyTags);
            

            foreach (GameObject enemy in enemies) {
                if (enhancement.explodeFanAngle > 0)
                {
                    if (!CheckFanArea(transform, enemy.transform, enhancement.explodeFanAngle, enhancement.explodeRadius)) {
                        continue;
                    }
                }

                Role enemyRole = enemy.GetComponent<Role>();
                float distance = Distance(contactPoint.point, enemy.transform.position);
                float centerRadius = 0.33f * enhancement.explodeRadius;
                float percentage = Mathf.Min(1.0f, (enhancement.explodeRadius - distance) / (enhancement.explodeRadius - centerRadius));
                float finalPercentage = percentage * (enhancement.explodeCenterDamage - enhancement.explodeEdgeDamage) + enhancement.explodeEdgeDamage;

                enemyRole.ReduceHealth(Damage(enemyRole, finalPercentage));

                if (enhancement.knockBackDistance > 0) {
                    Vector3 knockBackDirection = enemy.transform.position - contactPoint.point;
                    enemy.GetComponent<Rigidbody>().AddForce(knockBackDirection.normalized * enhancement.knockBackDistance * 1000);
                }
            }
        }
        //yield return null;
    }

    public DamageDef Damage(Role to, float extraModifier = 1) {
        //return Mathf.Round(damageModifier * finalDamage);
        DamageDef damageDef = damageUtils.CalculateBulletDamage(finalDamage, damageModifier * extraModifier, creatorRole, to, enhancement.damageType, enhancement.criticalAdder);
        return damageDef;
    }
    private GameObject[] GetEnemiesWithInRadius(float radius)
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag(TagMapping.Enemy.ToString());

        return allEnemies.Where(enemy => {
            float distance = Distance(gameObject, enemy);
            return distance <= radius;
        }).ToArray<GameObject>();
    }

    private float Distance(GameObject a, GameObject b) {
        return Distance(a.transform.position, b.transform.position);
    }
    private float Distance(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b);
    }


    public virtual void ProcessReturn()
    {
        if (!enhancement.returnComplete) {
            if (enhancement.returnTriggered)
            {
                if (enhancement.isReturnCaster && Distance(gameObject, creator) > 3)
                {
                    Vector3 returnDirection = creator.transform.position - transform.position;
                    bulletRigidbody.velocity = returnDirection.normalized * returnMagnitude;
                }
                else {
                    enhancement.returnComplete = true;
                }
            }
            else
            {
                float distance = enhancement.isReturnCaster ? Distance(gameObject, creator) : Distance(gameObject.transform.position, originalPoint);
                returnMagnitude = bulletRigidbody.velocity.magnitude;

                if (distance > enhancement.maxReturnDistance)
                {
                    Vector3 returnTargetPos = enhancement.isReturnCaster ? creator.transform.position : originalPoint;
                    Vector3 returnDirection = returnTargetPos - transform.position;
                    bulletRigidbody.velocity = returnDirection.normalized * returnMagnitude;
                    enhancement.returnTriggered = true;
                    ResetCollierIgnore();
                    damageModifier = damageModifier * (1 - enhancement.returnDecayedPercentage);
                }
            }
        }
        

    }

    public virtual void ProcessPhysicalEnhancement() {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        SphereCollider bulletCollider = GetComponent<SphereCollider>();

        rigidbody.mass = rigidbody.mass * enhancement.massMultiplier;

        //bulletCollider.radius = bulletCollider.radius * enhancement.sizeMultiplier;
        float scaleAmount = enhancement.sizeMultiplier - 1;
        SetScale(new Vector3(scaleAmount, scaleAmount, scaleAmount));

        bulletSpeed = bulletSpeed * enhancement.speedMultiplier;
    }

    public void SetScale(Vector3 scale)
    {
        ParticleSystem[] sys = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem s in sys)
        {
            MainModule mainModule = s.main;
            mainModule.scalingMode = ParticleSystemScalingMode.Hierarchy;
        }
        transform.localScale += scale;
    }

    public virtual void Awake()
    {
        hittedObjects = new List<GameObject>();
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameController");
        damageUtils = gameManager.GetComponentInChildren<DamageUtils>();
    }

    public virtual void Start()
    {
        bulletCollider = GetComponent<Collider>();
        if (creator != null) {
            Physics.IgnoreCollision(bulletCollider, creator.GetComponent<Collider>());
        }
        Physics.IgnoreLayerCollision(LayerMapping.Bullet, LayerMapping.Bullet);

        bulletRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldDestory)
        {
            Destroy(gameObject);
        }
        if (shouldDestoryImpect && impectEffect!= null) {
            ParticleSystem ps = impectEffect.GetComponent<ParticleSystem>();
            if (ps != null && !ps.IsAlive()) {
                Destroy(impectEffect);
            }
        }
        if ((autoDestroyTime -= Time.deltaTime) <= 0) {
            Destroy(gameObject);
        }
        // fix slow bullet 
        if (bulletRigidbody.velocity.magnitude < 10) {
            //Debug.Log("Speed low, bump up " + bulletRigidbody.velocity.magnitude);
            if (bulletRigidbody.velocity.x == 0 && bulletRigidbody.velocity.y ==0 && bulletRigidbody.velocity.z == 0)
            {
                bulletRigidbody.velocity = direction;
            }
            bulletRigidbody.velocity = bulletRigidbody.velocity.normalized * 10f;
        }
        if (enhancement != null && enhancement.isReturn) {
            ProcessReturn();
        }
    }


    // TOOD Calculate
    //public float CalculateOnHitDamage(GameObject hit) {
    //    if (hit.GetComponent<Role>() != null) {
    //        return creatorRole.attribute.attack;
    //    }
    //    return 0;
    //}

    public void AddTracking()
    {
        if (enhancement.isTracking) {
            SmartMissile3D smartMissile = gameObject.AddComponent<SmartMissile3D>();
            smartMissile.m_lifeTime = autoDestroyTime;
            //smartMissile.m_searchRange = 10f; 
            smartMissile.m_searchAngle = enhancement.trackingAngle;
            smartMissile.m_canLooseTarget = true;
            smartMissile.m_guidanceIntensity = enhancement.trackingIntensity;
            // TODO multiple targets
            smartMissile.TargetTag = creatorRole.GetEnemyTags()[0];
            //smartMissile.m_targetOffset = m_config.m_targetOffset;
            //smartMissile.m_distanceInfluence = m_config.m_selectedPreset;
        }
    }
    private void StopTracking() {
        if (smartMissile != null)
        {
            smartMissile.TargetTag = "StopTracking";
        }
    }
    private void RestarTracking() {
        if (smartMissile != null)
        {
            smartMissile.TargetTag = creatorRole.GetEnemyTags()[0];
        }
    }

    public bool CheckFanArea(Transform from, Transform to, float angle, float radius)
    {
        Vector3 deltaA = to.position - from.position;

        float tmpAngle = Mathf.Acos(Vector3.Dot(deltaA.normalized, from.forward)) * Mathf.Rad2Deg;

        if (tmpAngle < angle * 0.5f && deltaA.magnitude < radius)
        {
            return true;
        }
        return false;
    }
}
