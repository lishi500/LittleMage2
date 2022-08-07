using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Role
{
    [Header("Enemy")]
    public string bulletName;

    public override string GetBulletName()
    {
        return bulletName;
    }

    public override void Awake()
    {
        base.Awake();
        roleType = RoleType.EnemyMinion;
        if (attribute.exp == 0) {
            attribute.exp = 30;
        }

      
    }
}
