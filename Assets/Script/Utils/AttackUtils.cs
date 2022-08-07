using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackUtils : Singleton<AttackUtils>
{
    public List<Transform> FanAreaCollsion(Transform from, float angle, float distance, string[] tags) {
        List<Transform> transformList = new List<Transform>();
        float rayAngle = GetRayAngle(distance);
        //Debug.Log("FanAreaCollsion ----------------------- ");
        for (int i = 0; i < angle / rayAngle + 1; i++)
        {
            float nextAngle = ( i * rayAngle > angle ? angle : i * rayAngle) - (angle / 2);

            Vector3 targetPos = GetPositionWithDistanceAndAngle(from, distance, nextAngle);
            Vector3 targetDir = targetPos - from.position;
            Debug.DrawRay(from.position, targetDir, Color.yellow, 0.5f);

            RaycastHit[] hits;
            hits = Physics.RaycastAll(from.position, targetDir, 100.0F);
            for (int j = 0; j < hits.Length; j++)
            {
                RaycastHit hit = hits[j];
                if (tags.Contains(hit.transform.tag) && !transformList.Contains(hit.transform))
                {
                    //Debug.Log("Fan " + hit.transform.name);
                    transformList.Add(hit.transform);
                }
            }
        }

        return transformList;
    }

    public List<Transform> CircleCollision(Vector3 center, float radius, string[] tags) {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);

        return hitColliders
            .Where(collider => { return tags.Contains(collider.tag); })
            .Select(collider => collider.transform)
            .ToList();
    }

    //public List<Transform> RayCollision(Transform start, float distance, float width = 0, float angle = 0) {
    //    int numberOfLines = (int)Mathf.Ceil(width / 0.5f) + 1;
    //    float startWidth = width / 2 - width;

    //    while (numberOfLines > 0) {
    //        float widthOffset = 0.5f * numberOfLines + startWidth;

    //    }
    //}

    public void KnockBack(Vector3 forcePoint, Transform target, float distance = 5) {
        Rigidbody rigidbody = target.GetComponent<Rigidbody>();
        if (rigidbody != null) {
            Vector3 moveDirection = forcePoint - target.transform.position;
            target.GetComponent<Rigidbody>().AddForce(moveDirection.normalized * - (distance * 100));
        }
    }

    private float GetRayAngle(float distance) {
        return distance < 10 ? 5 : 3;
    }

    private Vector3 GetPositionWithDistanceAndAngle(Transform from, float distance, float angle)
    {
        Vector3 newPos = from.position + Quaternion.AngleAxis(angle, Vector3.up) * from.forward * distance;
        return newPos;
    }

}
