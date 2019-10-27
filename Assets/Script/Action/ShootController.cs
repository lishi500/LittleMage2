using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootController : MonoBehaviour
{
    BulletUtils bulletUtils;
    public void Shoot(GameObject owner, Transform shootPoint, Transform target) {
        GameObject bullet = bulletUtils.CreateBullet(shootPoint, owner, GetReceivePoint(target).position, target, "Fire");
        BulletEnhancement enhancement = bullet.GetComponent<Bullet>().enhancement;
    }

    public void ShootOrb(GameObject orbBullet, GameObject owner, Transform shootPoint, Transform target)
    {
        GameObject bullet = bulletUtils.CreateOrbBullet(Instantiate(orbBullet), shootPoint, owner, GetReceivePoint(target).position, target);
        BulletEnhancement enhancement = bullet.GetComponent<Bullet>().enhancement;

        ShootMultiAndDartleBullet(orbBullet, owner, shootPoint, target, enhancement);

        if (enhancement.dartleBulletNumber == 0) {
            Destroy(orbBullet);
        }
    }

    public void ShootMultiAndDartleBullet(GameObject orbBullet, GameObject owner, Transform shootPoint, Transform target, BulletEnhancement enhancement) {
        float damageModifier = CalculateDecayedDamagePercentage(enhancement);
        if (enhancement.dartleBulletNumber > 0)
        {
           StartCoroutine(ShootDartleBullet(orbBullet, owner, shootPoint, target, enhancement, damageModifier));
        } else if (enhancement.multiBulletNumber > 0) {
            ShootMultiBullet(orbBullet, owner, shootPoint, target, enhancement, damageModifier);
        }

    }
    public IEnumerator ShootDartleBullet(GameObject orbBullet, GameObject owner, Transform shootPoint, Transform target, BulletEnhancement enhancement, float damageModifier) {
        int count = enhancement.dartleBulletNumber;

        while (count > 0) {
            yield return new WaitForSeconds(0.15f);

            if (enhancement.multiBulletNumber > 0)
            {
                ShootMultiBullet(orbBullet, owner, shootPoint, target, enhancement, damageModifier);
            }
            else {
                GameObject bulletObj = Instantiate(orbBullet);
                bulletObj.GetComponent<OrbController>().DestoryHandEffect();
                bulletUtils.CreateOrbBullet(bulletObj, shootPoint, owner, GetReceivePoint(target).position, target);
            }
            count--;
        }
        Destroy(orbBullet);
        yield return null;
    }

    public void ShootMultiBullet(GameObject orbBullet, GameObject owner, Transform shootPoint, Transform target, BulletEnhancement enhancement, float damageModifier) {
        int count = enhancement.multiBulletNumber;
        float totalAngle = count > 5 ? 120 : 90;
        for (int i = 0; i < count; i++) {
            float angle = getNextShootAngle(totalAngle);
            Vector3 targetPos = GetPositionWithDistanceAndAngle(shootPoint, 10f, angle);

            GameObject bulletObj = Instantiate(orbBullet);
            bulletObj.GetComponent<OrbController>().DestoryHandEffect();
            bulletUtils.CreateOrbBullet(bulletObj, shootPoint, owner, targetPos, target);
        }
    }

    private float CalculateDecayedDamagePercentage(BulletEnhancement enhancement) {
        float multiPercentage = Mathf.Pow(1 - enhancement.multiDecayedPercentage, enhancement.multiBulletNumber);
        float dartlePercentage = Mathf.Pow(1 - enhancement.dartleDecayedPercentage, enhancement.dartleBulletNumber);
        return Mathf.Max(multiPercentage * dartlePercentage, enhancement.dartleMinPercentage);
    }
    private Transform GetReceivePoint(Transform target)
    {
        Transform receivePoint = target.Find("ReceivePoint");
        return receivePoint != null ? receivePoint : target;
    }
    float getNextShootAngle(float totalAngle)
    {
        float rndFloat = RandomFromDistribution.RandomRangeNormalDistribution(0, 1, RandomFromDistribution.ConfidenceLevel_e._80);

        float nextAngle = totalAngle * rndFloat - totalAngle / 2;
        return nextAngle;
    }
    private Vector3 GetPositionWithDistanceAndAngle(Transform from, float distance, float angle)
    {
        Vector3 newPos = from.position + Quaternion.AngleAxis(angle, Vector3.up) * from.forward * distance;
        return newPos;
    }

    /* public void Shoot(Transform start, Transform target) {
        start.transform.LookAt(target);

        GameObject Bullet = (GameObject)Object.Instantiate(Resources.Load("Prafabs/Bullet1" ));
        Bullet.transform.position = start.position;
        Bullet.transform.rotation = start.rotation;
        // Bullet.GetComponent<Rigidbody>().isKinematic = false;
        Bullet.GetComponent<Collider>().enabled = true;
        Bullet.SetActive(true);

        Bullet.GetComponent<Bullet>().bulletSpeed = 1.0f;
        Bullet.transform.LookAt(target);
        Bullet.GetComponent<Bullet>().direction = start.position - target.position;

        //GameObject projectile = Instantiate(projectiles[currentProjectile], spawnPosition.position, Quaternion.identity) as GameObject;
        //projectile.transform.LookAt(hit.point);
        //projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward* speed);
        //projectile.GetComponent<MagicProjectileScript>().impactNormal = hit.normal;
    } */

    private void Awake()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameController");
        bulletUtils = gameManager.GetComponentInChildren<BulletUtils>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
