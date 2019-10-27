using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderEventHelper : MonoBehaviour
{
    public delegate void OnCollisionEnterDelegate(GameObject self, Collision collision);
    public event OnCollisionEnterDelegate notifyCollisionEnter;

    public delegate void OnTriggerEnterDelegate(GameObject self, Collider collide);
    public event OnTriggerEnterDelegate notifyTriggerEnter;


    public HashSet<int> hitList;
    public float timeToLive = 0.2f;
    public Transform from;

    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("OnCollisionEnter Detected");
        if (notifyCollisionEnter != null)
        {
            int id = collision.gameObject.GetInstanceID();
            if (!hitList.Contains(id))
            {
                hitList.Add(id);
                notifyCollisionEnter(gameObject, collision);
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        //Debug.Log("OnCollisionEnter Detected" + collider.gameObject.name);
        if (notifyTriggerEnter != null)
        {
            notifyTriggerEnter(gameObject, collider);
        }
    }


    void Start()
    {
        hitList = new HashSet<int>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeToLive > 0)
        {
            timeToLive -= Time.deltaTime;
            if (from != null) {
                Debug.DrawLine(from.position, gameObject.transform.position, Color.red);
            }
        }
        else {
            Destroy(gameObject);
        }
    }
}
