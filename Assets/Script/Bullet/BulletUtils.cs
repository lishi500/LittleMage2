using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletUtils : MonoBehaviour
{

    PrafabHolder prafabHolder;
    Dictionary<BulletSize, float> bulletColliderRadius;
    GameManager gameManager;

    private Vector3 gizmosShootPoint;
    private Vector3 gizmosReceivePoint;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(gizmosReceivePoint, 0.5f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(gizmosShootPoint, 0.5f);
    }

    public GameObject CreateOrbBullet(GameObject orbBulletObj, Transform start, GameObject owner, Vector3 targetPos, Transform target, float damageModifier = 1f)
    {
        OrbController orbController = orbBulletObj.GetComponent<OrbController>();
        DistanceDestroy distanceDestroy = orbBulletObj.GetComponent<DistanceDestroy>();
        SphereCollider bulletCollider = orbBulletObj.AddComponent<SphereCollider>();
        OrbBullet bullet = orbBulletObj.AddComponent<OrbBullet>();
        Rigidbody rigidbody = orbBulletObj.AddComponent<Rigidbody>();
        OrbSkill skill = orbController.skill;
        Role ownerRole = owner.GetComponent<Role>();

        ownerRole.GetSkillByName(skill.skillName).StartCD();

        gizmosShootPoint = orbBulletObj.transform.position;
        Vector3 scale = start.localScale;
        orbBulletObj.transform.position = start.position;
        orbBulletObj.transform.rotation = start.rotation;
        orbBulletObj.transform.LookAt(target);
        orbBulletObj.transform.localScale = scale;
        gizmosShootPoint = orbBulletObj.transform.position;

        distanceDestroy.fixPosition = start.position;

        float raduis;
        bulletColliderRadius.TryGetValue(BulletSize.Mega, out raduis);
        bulletCollider.radius = raduis;
        bulletCollider.enabled = true;

        rigidbody.mass = 2;
        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;

        bullet.orbSkill = skill;
        bullet.bulletSpeed = normalizeAttackSpeed(ownerRole.attribute.attackSpeed);
        bullet.creator = owner;
        bullet.damageModifier = damageModifier;

        bullet.direction = start.position - targetPos;

        GameObject bulletprojectile = Instantiate(skill.bulletPrafab);
        bulletprojectile.transform.SetParent(orbBulletObj.transform, false);

        GameObject bulletImpactPrafab = skill.bulletImpactPrafab;
        bullet.impectPrafab = bulletImpactPrafab;

        bullet.autoDestroyTime = 10;

        bullet.enhancement = GetBulletEnhancement();
        bullet.EnhancementMerge();

        bullet.PreEnhancementProcess();
        bullet.ApplyForce();

        orbController.caster = owner.GetComponent<Role>();
        orbController.OrbAttackStart();

        return orbBulletObj;
    }
    public GameObject CreateOrbBullet(GameObject orbBulletObj, Transform start, GameObject owner, Transform target) {
        Vector3 targetPos = target.GetComponent<Role>().GetShootPoint(ShootPointPosition.RECEIVE).position;
        return CreateOrbBullet(orbBulletObj, start, owner, targetPos, target, 1f);
    }

    public GameObject CreateBullet(Transform start, GameObject owner, Vector3 targetPos, Transform target, string name, BulletSize size = BulletSize.Small) {
        Role ownerRole = owner.GetComponent<Role>();

        GameObject bulletObj = PrafabUtils.Instance.create(prafabHolder.Bullet);
        Vector3 scale = start.localScale;
        bulletObj.transform.position = start.position;
        bulletObj.transform.rotation = start.rotation;
        bulletObj.transform.LookAt(target);
        bulletObj.transform.localScale = scale;
        gizmosShootPoint = bulletObj.transform.position;


        SphereCollider bulletCollider = bulletObj.GetComponent<SphereCollider>();
        float radius;
        bulletColliderRadius.TryGetValue(size, out radius);
        bulletCollider.radius = radius;

        Bullet bullet = bulletObj.GetComponent<Bullet>();

        bullet.bulletSpeed = normalizeAttackSpeed(ownerRole.attribute.attackSpeed);
        bullet.creator = owner;
        bullet.aimingTarget = target.gameObject;

        bullet.direction = start.position - targetPos;
        gizmosReceivePoint = targetPos;

        GameObject bulletprojectilePrafab = prafabHolder.GetBullet(name, size);
        GameObject bulletImpectPrafab = prafabHolder.GetBulletImpect(name, size);

        GameObject bulletprojectile = PrafabUtils.Instance.create(bulletprojectilePrafab);
        bulletprojectile.transform.SetParent(bulletObj.transform, false);


        bullet.enhancement = GetBulletEnhancement();
        bullet.impectPrafab = bulletImpectPrafab;

        bullet.PreEnhancementProcess();
        bullet.ApplyForce();

        return bulletObj;
    }

    private BulletEnhancement GetBulletEnhancement() {
       return Instantiate(prafabHolder.defaultBulletEnhancement);
    }

    private Transform GetReceivePoint(Transform target) {
        Transform receivePoint = target.Find("ReceivePoint");
        return receivePoint != null ? receivePoint : target;
    }

    private float normalizeAttackSpeed(float attackSpeed)
    {
        //return (attackSpeed / (attackSpeed + 1.0f)) * 2.5f;
        return attackSpeed / 10 + 1;
    }
    private void Awake()
    {
        GameObject gameManagerObj = GameObject.FindGameObjectWithTag("GameController");
        prafabHolder = gameManagerObj.GetComponent<PrafabHolder>();
        gameManager = gameManagerObj.GetComponent<GameManager>();
        bulletColliderRadius = new Dictionary<BulletSize, float>();
        bulletColliderRadius.Add(BulletSize.Tiny, 0.3f);
        bulletColliderRadius.Add(BulletSize.Small, 0.45f);
        bulletColliderRadius.Add(BulletSize.Normal, 0.6f);
        bulletColliderRadius.Add(BulletSize.Mega, 0.85f);

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   
}
