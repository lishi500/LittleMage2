using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceDestroy : MonoBehaviour
{
    public Vector3 fixPosition;
    public Transform dynamicPosition;
    public float destroyDistance;
    private float distance;

    void Start()
    {
        distance = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (dynamicPosition != null)
        {
            distance = Vector3.Distance(transform.position, dynamicPosition.position);
        }
        else {
            distance = Vector3.Distance(transform.position, fixPosition);
        }
        if (distance >= destroyDistance) {
            if (notifyDistanceDestroy != null) {
                notifyDistanceDestroy(gameObject);
            }

            Destroy(gameObject);
        } 
    }

    public delegate void DistanceDestroyDelegate(GameObject obj);
    public event DistanceDestroyDelegate notifyDistanceDestroy;
}
