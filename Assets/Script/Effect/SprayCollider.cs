using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;


public class SprayCollider : MonoBehaviour
{
    public float maxDistance;
    public float angle;

    public float fireSpeed;
    public float colliderWidth;
    public float distributeFactor; // 0: evenly - 1: Normal distribution
    public float density; // number of bullet per second
    public float maxTriggerFrequency; // 0: trigger every time, 0.5 every 0.5s

    public bool isRay = false;
    public bool isTrigger = false;
    public bool isDistoryAfterTrigger = false;
    public List<string> colliderTags;

    public RandomFromDistribution.ConfidenceLevel_e conf_level = RandomFromDistribution.ConfidenceLevel_e._95;

    public delegate void CollideDelegate(GameObject selfObj, GameObject targetObj);
    public CollideDelegate onCollideNotify;

    [HideInInspector]
    public Transform start;
    [HideInInspector]
    public Transform target;
    //public bool isStart;

    private int alreadyGeneratedBulletCount;
    private float timePasted;
    private Dictionary<int, float> triggeredMap;
    private PrafabHolder prafabHolder;

    public void OnCollision(GameObject selfObject, Collision collision)
    {
        if (isTagMatches(collision.gameObject.tag)) {
            NotifyCollision(selfObject, collision.gameObject);
        }
    }

    public void OnTrigger(GameObject selfObject, Collider collider)
    {
        if (isTagMatches(collider.gameObject.tag))
        {
            NotifyCollision(selfObject, collider.gameObject);
        }
    }
 
    float getNextShootAngle()
    {
        float rndFloat = RandomFromDistribution.RandomRangeNormalDistribution(0, 1, conf_level);
        float nextAngle = angle * rndFloat - angle / 2;
        //Debug.Log(nextAngle);
        return nextAngle;
    }
   
    void generateTriggerObject(int count)
    {
        for (int i = 0; i < count; i++) {
            alreadyGeneratedBulletCount++;

            GameObject SprayColliderBullet = Instantiate(prafabHolder.SprayColliderBullet, start.transform.position, Quaternion.identity);
            RegisterBulletListener(SprayColliderBullet);

            SphereCollider collider = SprayColliderBullet.GetComponent<SphereCollider>();
            collider.radius = colliderWidth / 2;
            collider.isTrigger = isTrigger;

            float nextAngle = getNextShootAngle();
            Vector3 targetPos = GetPositionWithDistanceAndAngle(start, maxDistance, nextAngle);
            Vector3 direction = start.position - targetPos;

            SprayColliderBullet.transform.rotation = start.rotation;
            SprayColliderBullet.transform.LookAt(targetPos);

            SprayColliderBullet.GetComponent<Rigidbody>().AddForce(-direction * fireSpeed * 10);

            DistanceDestroy distanceDestroy = SprayColliderBullet.GetComponent<DistanceDestroy>();
            distanceDestroy.destroyDistance = maxDistance;
            distanceDestroy.dynamicPosition = start;

        }
    }

    Vector3 targetPos2;
    Vector3 targetStart;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetPos2, 1);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(targetStart, 1);
    }
    void generateRayLine(int count)
    {
        //Debug.Log("Generate ray" + count);
        for (int i = 0; i < count; i++)
        {
            alreadyGeneratedBulletCount++;
            float nextAngle = getNextShootAngle();
            Vector3 targetPos = GetPositionWithDistanceAndAngle(start, maxDistance, nextAngle);
            Vector3 targetDir = targetPos - start.transform.position;
            targetPos2 = targetPos;
            targetStart = start.transform.position;
            Debug.DrawRay(start.transform.position, targetDir, Color.yellow);

            RaycastHit[] hits;
            hits = Physics.RaycastAll(start.position, targetDir, 100.0F);
            //Debug.Log("Hit ray count: " + hits.Length);
            for (int j = 0; j < hits.Length; j++)
            {
                RaycastHit hit = hits[j];
                if (isTagMatches(hit.transform.tag)) {
                    if (CheckTriggeredMap(hit.transform.GetInstanceID())) {
                        NotifyCollision(gameObject, hit.transform.gameObject);
                        //Debug.Log("Ray " + hit.transform.gameObject.name + "  " + hit.transform.GetInstanceID() + " " + timePasted + "  " + triggeredMap.ContainsKey(hit.transform.GetInstanceID()));
                    }
                }
            }
        }

    }




    void ShootTriggers()
    {
        int expectedBulletCount = (int) Math.Ceiling(density * timePasted);
        if (expectedBulletCount > alreadyGeneratedBulletCount)
        {
            int nextCount = expectedBulletCount - alreadyGeneratedBulletCount;
            if (isRay)
            {
                generateRayLine(nextCount);
            }
            else
            {
                generateTriggerObject(nextCount);
            }
        }
    }

    void ClearTiggerMap()
    {
        if (maxTriggerFrequency > 0)
        {
            List<int> removals = new List<int>();

            foreach (KeyValuePair<int, float> entry in triggeredMap)
            {
                if (entry.Value + maxTriggerFrequency <= timePasted)
                {
                    removals.Add(entry.Key);
                }
            }
            foreach (int key in removals)
            {
                triggeredMap.Remove(key);
            }
        }
    }

    void Start()
    {
        triggeredMap = new Dictionary<int, float>();
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameController");
        prafabHolder = gameManager.GetComponent<PrafabHolder>();
    }

    void Update()
    {
        //if (isStart)
        //{
            timePasted += Time.deltaTime;
            ClearTiggerMap();
            ShootTriggers();
        //}
    }

    private Vector3 GetPositionWithDistanceAndAngle(Transform from, float distance, float angle)
    {
        Vector3 newPos = from.position + Quaternion.AngleAxis(angle, Vector3.up) * from.forward * distance;
        return newPos;
    }

    private bool CheckTriggeredMap(int id) {
        if (triggeredMap.ContainsKey(id)) {
            return false;
        }
        triggeredMap.Add(id, timePasted);

        return true;
    }
    private void NotifyCollision(GameObject selfObj, GameObject targetObj)
    {
        if (onCollideNotify != null)
        {
            onCollideNotify(selfObj, targetObj);
        }
    }
    private void RegisterBulletListener(GameObject SprayColliderBullet)
    {
        ColliderEventHelper eventHelper = SprayColliderBullet.GetComponent<ColliderEventHelper>();
        eventHelper.from = start;
        if (isTrigger)
        {
            eventHelper.notifyTriggerEnter += OnTrigger;
        }
        else
        {
            eventHelper.notifyCollisionEnter += OnCollision;
        }
    }
    private bool isTagMatches(string tag) {
        return colliderTags.Contains(tag);
    }
}

