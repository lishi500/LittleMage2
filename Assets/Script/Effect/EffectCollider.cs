using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EffectCollider
{
    public ColliderType type;
    public PositionType position;
    public Vector3 center;
    // For Sphere
    public float radius;
    // For Box
    Vector3 scale;

    public float delay;
    public float duration = 0.2f;

    public bool isTrigger = true;

}
