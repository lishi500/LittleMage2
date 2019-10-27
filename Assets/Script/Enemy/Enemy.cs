using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Role
{
    [Header("Enemy")]
    public string bulletName; 

    void Start()
    {
        alignment = AlignmentType.Enemy;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
