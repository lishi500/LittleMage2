using SensorToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class FireSpraySkill : ActiveSkill
{
    [Header("Collider")]
    public float maxDistance;
    public float density;
    public List<string> colliderTags;

    public float sprayAngleWidth = 20;
    public float sprayAngleHeight = 20;
    public float triggerFrequency = 0.25f;
    TriggerSensor triggerSensor;

    private readonly float fireSpeed = 10f;
    private readonly float angle = 25;
    private readonly float colliderWidth = 0.25f;
    private readonly bool isTrigger = false;
    private readonly bool isDestoryAfterTrigger = false;

    private float iteratedTime = 0;


    public override void SkillSetup()
    {
        MeshCollider meshCollider = skillControllerObj.GetComponent<MeshCollider>();
        triggerSensor = skillControllerObj.GetComponent<TriggerSensor>();
        FOVCollider fovCollider = skillControllerObj.GetComponent<FOVCollider>();

        fovCollider.Length = maxDistance;
        fovCollider.FOVAngle = sprayAngleWidth;
        fovCollider.ElevationAngle = sprayAngleHeight;

        triggerSensor.EnableTagFilter = true;
        triggerSensor.AllowedTags = colliderTags.ToArray();
        triggerSensor.DetectionMode = SensorMode.RigidBodies;

        fovCollider.enabled = true;
        triggerSensor.enabled = true;
        meshCollider.enabled = true;
    }

    public override void UpdateCollider()
    {
        triggerSensor = skillControllerObj.GetComponent<TriggerSensor>();
        //+triggerSensor.DetectedObjects.GetEnumerator().
        iteratedTime += Time.deltaTime;
        if (iteratedTime >= triggerFrequency) {
            iteratedTime = 0;
            IEnumerator<GameObject> detectedEnemies = triggerSensor.DetectedObjects.GetEnumerator();
            while (detectedEnemies.MoveNext()) {
                GameObject enemy = detectedEnemies.Current;
                SprayHit(enemy);
                Debug.Log("Update Collider ");

            }
        }
    }

    private void SprayHit(GameObject targetObject)
    {
        Role role = targetObject.GetComponent<Role>();

        if (role != null)
        {
            role.ReduceHealth(CalculateValue());
            ApplyBuffsToRole(triggeredBuffDefs, role);
            skillControllerObj.GetComponent<SkillController>().ShowBaseEffect(OnTriggerEffect, role.transform.position);
        }
    }

    public void SkillSetup2()
    {
        SprayCollider sprayCollider = skillControllerObj.GetComponent<SprayCollider>();

        sprayCollider.maxDistance = maxDistance;
        sprayCollider.density = density; // number of bullet per second

        sprayCollider.angle = angle;
        sprayCollider.fireSpeed = fireSpeed;
        sprayCollider.colliderWidth = colliderWidth;
        sprayCollider.distributeFactor = 1; // 0: evenly - 1: Normal distribution
        sprayCollider.maxTriggerFrequency = triggerFrequency; // 0: trigger every time, 0.5 every 0.5s
        sprayCollider.conf_level = RandomFromDistribution.ConfidenceLevel_e._99;
        sprayCollider.isRay = true;
        sprayCollider.isTrigger = isTrigger;
        sprayCollider.isDistoryAfterTrigger = isDestoryAfterTrigger;
        sprayCollider.colliderTags = colliderTags;

        sprayCollider.start = skillControllerObj.GetComponent<SkillController>().GeneratePositionByType(PositionType.SHOOT_POINT);
        sprayCollider.onCollideNotify += OnSprayHit;

        sprayCollider.enabled = true;
    }

    public void OnSprayHit(GameObject bullet, GameObject targetObject) {
        Role role = targetObject.GetComponent<Role>();

        if (role != null) {
            role.ReduceHealth(CalculateValue());
            ApplyBuffsToRole(triggeredBuffDefs, role);
            skillControllerObj.GetComponent<SkillController>().ShowBaseEffect(OnTriggerEffect, role.transform.position);
        }
    }

}
