using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Basic Camera Settings")]
    public float followSpeed = 4.0f;
    public GameObject TargetToFollow;

    // TargetHeight dynamic height base on the size of map, pending to do, if needed

    // shake, screen shake, if needed

    void Start()
    {
        if (TargetToFollow == null) {
            TargetToFollow = GameObject.FindGameObjectWithTag("Player");
        }
    }

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, TargetToFollow.transform.position, followSpeed * Time.deltaTime);
    }
}
