using ECM.Controllers;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIUtils : MonoBehaviour
{
    private BulletUtils bulletUtils;
    private BaseAgentController GetAgentController(Transform transform)
    {
        return transform.GetComponent<BaseAI>().baseAgentController;
    }
    public NavMeshAgent GetNavMeshAgent(Transform transform) {
        return GetAgentController(transform).agent;
    }

    public bool CanAttack(GameObject from, GameObject to, float attackDistance, bool ignoreObstacle = false) {
        float distance = DistanceBetweenObject(from, to);
        if (ignoreObstacle) {
            return distance < attackDistance;
        }

        bool canSee = CanSeeObject(from, to);
        //Debug.Log("distance: " + distance + " cansee " + canSee);
        return distance < attackDistance && canSee;
    }
    public float DistanceBetweenObject(GameObject obj1, GameObject obj2) {
        return Vector3.Distance(obj1.transform.position, obj2.transform.position);
    }

    public Vector3 DistanceToPosition(Vector3 from, Vector3 to, float stopDistance) {
        Vector3 direction = to - from;
        return from + (direction - (direction.normalized * stopDistance));
    }

  /*  public bool CanSeeObject(GameObject from, GameObject to) {
        RaycastHit hit;
        if (Physics.Linecast(from.transform.position, to.transform.position, out hit))
        {
            if (hit.transform == to.transform || hit.transform.IsChildOf(to.transform) || to.transform.IsChildOf(hit.transform))
            {
                return true;
            }
        }
    }*/

    public bool CanSeeObject(GameObject from, GameObject target)
    {
        Transform shootPoint = from.GetComponent<Role>().GetShootPoint(ShootPointPosition.MID);;
        Transform receivePoint = target.GetComponent<Role>().GetShootPoint(ShootPointPosition.RECEIVE);

        Vector3 fromSensorPos = shootPoint == null ? from.transform.position : shootPoint.transform.position;
        Vector3 targetReceivePos = receivePoint == null ? target.transform.position : receivePoint.transform.position;
        Vector3 targetDir = targetReceivePos - fromSensorPos;

        RaycastHit hit;
        if (Physics.Raycast(fromSensorPos, targetDir, out hit))
        {
            //if ( hit.collider.CompareTag(TagMapping.Player.ToString()))
            if (hit.collider.gameObject == target)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsWalkable(Vector3 position) {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(position, out hit, 1, 1 << NavMesh.GetAreaFromName("Walkable"))) {
            // Debug.Log("Walkable");
            return true;
        }
        // Debug.Log("Can not Walke");
        return false;
    }

    public void FaceAtTarget(Transform from, Transform to) 
    {
        FaceAtTarget(from, to.position);
    }
    public void FaceAtTarget(Transform from, Vector3 to)
    {
        Vector3 targetDir = to - from.position;
        GetAgentController(from).RotateTowards(targetDir);
    }

    public GameObject GetNearestObject(List<GameObject> objectList, GameObject source)
    {
        if (objectList.Count == 0)
        {
            //Debug.Log("No visiable enemy found");
            return null;
        }

        float shortestDistance = float.MaxValue;
        int nearstIndex = -1;

        for (int i = 0; i < objectList.Count; i++)
        {
            float distance = Vector3.Distance(gameObject.transform.position, objectList[i].transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearstIndex = i;
            }
        }


        return objectList[nearstIndex];
    }

    public List<GameObject> GetVisiableObject(List<GameObject> objectList, GameObject source) {
        return objectList.Where(obj => CanSeeObject(source, obj)).ToList();
    }

    public bool ChasingTarget(Transform from, Transform target, float stopDistance) {
        NavMeshAgent mNavMeshAgent = GetNavMeshAgent(from);
        //Vector3 endPosition = target.TransformPoint(Vector3.right * + stopDistance);

        mNavMeshAgent.stoppingDistance = stopDistance;
        mNavMeshAgent.SetDestination(target.position);

        StartCoroutine(Checkdd(mNavMeshAgent));

        return HasReachDestination(mNavMeshAgent);
    }

    private IEnumerator Checkdd(NavMeshAgent mNavMeshAgent)
    {
        while (true)
        {
            bool hasPath = mNavMeshAgent.hasPath;

            //Debug.Log("SetDestination:" + hasPath);

            yield return new WaitForSeconds(0.01f);
            if (hasPath) {
                break;
            }
        }

        yield return null;
    }

    public bool HasReachDestination(NavMeshAgent mNavMeshAgent) {
        if (!mNavMeshAgent.pathPending)
        {
            if (mNavMeshAgent.remainingDistance <= mNavMeshAgent.stoppingDistance)
            {
                if (!mNavMeshAgent.hasPath || mNavMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool MoveToDestination(Transform from, Vector3 position, float stopDistance = 1f) {
        NavMeshAgent mNavMeshAgent = GetNavMeshAgent(from);

        mNavMeshAgent.stoppingDistance = stopDistance;
        mNavMeshAgent.SetDestination(position);
        //Debug.Log("Distance left " + mNavMeshAgent.remainingDistance);

        return HasReachDestination(mNavMeshAgent);
    }

    public Vector3 GenerateRandomPosition(Transform from, int distanceMin = 8, int distanceMax = 12) {
        bool walkable = false;
        int maxTry = 20;

        while (!walkable && maxTry > 0) {
            int distance = Random.Range(distanceMin, distanceMax);
            int angle = Random.Range(0, 360);
            Vector3 newPos = GetPositionWithDistanceAndAngle(from, distance, angle);
            if (IsWalkable(newPos)) {
                return newPos;
            }
            maxTry--;
        }
        return from.position;
    }
    public Vector3 GetPositionWithDistanceAndAngle(Transform from, float distance, float angle) {
        Vector3 newPos = transform.position + Quaternion.AngleAxis(angle, Vector3.right) * transform.forward * distance;
        return newPos;
    }

    public Vector3 FindHiddenPoint(Transform from, Transform enemy) {
        // TODO 
        return new Vector3(0, 0, 0);
    }

    public Transform GetShootPoint(Transform from) {
        return from.Find("ShootPoint");
    }

    public bool CheckFanArea(Transform from, Transform to, float angle, float radius) {
        Vector3 deltaA = to.position - from.position;

        float tmpAngle = Mathf.Acos(Vector3.Dot(deltaA.normalized, from.forward)) * Mathf.Rad2Deg;

        if (tmpAngle < angle * 0.5f && deltaA.magnitude < radius)
        {
            return true;
        }
        return false;
    }

    public void Shoot(GameObject owner, Transform shootPoint, Transform target, string bulletName)
    {
        float attackSpeed = owner.GetComponent<Role>().attribute.attackSpeed;
        Role targetRole = target.GetComponent<Role>();
        GameObject bullet = bulletUtils.CreateBullet(shootPoint, owner, targetRole.GetShootPoint(ShootPointPosition.RECEIVE).position, target, bulletName);
    }

    public Vector3 CalculateBezierPoint(float t, Vector3[] p) {
        return CalculateBezierPoint(t, p[0], p[1], p[2], p[3]);
    }
    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) // returns the vector3 of the desired point along the curve. this vector3 will be the transform.position of the ship
    {
        float tt = t * t;
        float ttt = t * tt;
        float u = 1.0f - t;
        float uu = u * u;
        float uuu = u * uu;

        Vector3 B = new Vector3();
        B = uuu * p0;
        B += 3.0f * uu * t * p1;
        B += 3.0f * u * tt * p2;
        B += ttt * p3;

        return B;
    }

    public Vector3[] GenerateStandardAncherBezierPoint(Vector3 from, Vector3 to) {
        Vector3 midPoint = (from + to) / 2f;
        Vector3 secondPoint = (from + to) / 2f;
        secondPoint.y = secondPoint.y + 6;
        Vector3 thirdPoint = (((from + to) / 2f) + to ) / 2f;
        thirdPoint.y = thirdPoint.y + 6;

        return new Vector3[] { from, secondPoint, thirdPoint, to };
    }

    private float normalizeAttackSpeed(float attackSpeed)
    {
        return attackSpeed / 10 + 1;
    }

    void Start()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameController");
        bulletUtils = gameManager.GetComponentInChildren<BulletUtils>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
