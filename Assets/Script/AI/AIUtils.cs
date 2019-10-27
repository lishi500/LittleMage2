using ECM.Controllers;
using System.Collections;
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

    public bool CanAttack(GameObject from, GameObject to, float attackDistance) {
        float distance = DistanceBetweenObject(from, to);
        bool canSee = CanSeeObject(from, to);
        //Debug.Log("distance: " + distance + " cansee " + canSee);
        return distance < attackDistance && canSee;
    }
    public float DistanceBetweenObject(GameObject obj1, GameObject obj2) {
        return Vector3.Distance(obj1.transform.position, obj2.transform.position);
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
        Transform shootPoint = GetShootPoint(from.transform);
        Vector3 fromSensorPos = shootPoint!= null ? shootPoint.transform.position : from.transform.position;
        Vector3 targetDir = target.transform.position - fromSensorPos;

        RaycastHit hit;
        if (Physics.Raycast(fromSensorPos, targetDir, out hit))
        {
            if (hit.collider.CompareTag("Player"))
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
        Vector3 targetDir = to.position - from.position;
        GetAgentController(from).RotateTowards(targetDir);
    }

    public bool ChasingTarget(Transform from, Transform target, float stopDistance) {
        NavMeshAgent mNavMeshAgent = GetNavMeshAgent(from);

        //Vector3 endPosition = target.TransformPoint(Vector3.right * + stopDistance);

        mNavMeshAgent.stoppingDistance = stopDistance;
        mNavMeshAgent.SetDestination(target.position);

        return HasReachDestination(mNavMeshAgent);
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
