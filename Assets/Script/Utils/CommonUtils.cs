using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommonUtils : MonoBehaviour
{
    public GameObject[] GetObjectWithInRadius(Vector3 central, float radius, string tag)
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag(tag);

        return allEnemies.Where(enemy => {
            float distance = Distance(central, enemy.transform.position);
            return distance <= radius;
        }).ToArray<GameObject>();
    }

    private float Distance(GameObject a, GameObject b)
    {
        return Distance(a.transform.position, b.transform.position);
    }
    private float Distance(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b);
    }

    private Vector3 GetPositionWithDistanceAndAngle(Transform from, float distance, float angle)
    {
        Vector3 newPos = from.position + Quaternion.AngleAxis(angle, Vector3.up) * from.forward * distance;
        return newPos;
    }
    private void PlayParticleSystem(GameObject effect)
    {
        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
        }
    }

}
