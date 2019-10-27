using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    public GameObject creator;
    public GameObject primaryTarget;

    public Skill skill;

    protected PrafabHolder prafabHolder;
    protected CommonUtils commonUtils;

    [HideInInspector]
    public int effectChainIndex;
    [HideInInspector]
    public int colliderChainIndex;
    [HideInInspector]
    public float pastTime;
    [HideInInspector]
    public Transform position1;
    [HideInInspector]
    public Transform position2;
    [HideInInspector]
    public Vector3 direction;

    [HideInInspector]
    public bool isStarted = false;

    void ShowEffectChain(EffectChain effectChain, Vector3 position)
    {
        
        GameObject effect = null;
        if (effectChain.followRole)
        {
            effect = GameObject.Instantiate(effectChain.effect, position, Quaternion.identity, GeneratePositionByType(effectChain.positionType));
        }
        else {
            effect = GameObject.Instantiate(effectChain.effect, position, Quaternion.identity);
        }

        if (effectChain.useRoleFacing)
        {
            Vector3 targetPos = GetPositionWithDistanceAndAngle(creator.transform, 10, 0);
            effect.transform.LookAt(targetPos);
        }

        float destoryTime = effectChain.duration == 0 ? 3 : effectChain.duration;
        effect.AddComponent<AutoDestroy>().timeToLive = destoryTime;
      

        PlayParticleSystem(effect);
       
    }

    private Vector3 GetPositionWithDistanceAndAngle(Transform from, float distance, float angle)
    {
        Vector3 newPos = from.position + Quaternion.AngleAxis(angle, Vector3.up) * from.forward * distance;
        return newPos;
    }

    public void ShowBaseEffect(BaseEffect baseEffect, Vector3 position)
    {
        if (baseEffect.effect != null)
        {
            GameObject effectObj = GameObject.Instantiate(baseEffect.effect, position, Quaternion.identity);

            if (baseEffect.duration > 0)
            {
                effectObj.AddComponent<AutoDestroy>().timeToLive = baseEffect.duration;
            }

            PlayParticleSystem(effectObj);
        }
    }

    private void PlayParticleSystem(GameObject effect)
    {
        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
        }
    }
    public Transform GeneratePositionByType(PositionType type) {
        switch (type) {
            case PositionType.SELF:
                return creator.transform;
            case PositionType.SHOOT_POINT:
                Transform shootPoint = creator.GetComponent<Role>().GetShootPoint(skill.shootPointPosition);
                if (shootPoint != null) {
                    return shootPoint;
                }
                return creator.transform;
            case PositionType.TARGET:
                return primaryTarget.transform;
            default:
                return position1;
        }
    }

    void AddCollider(EffectCollider effectCollider, Vector3 position) {
        //Debug.Log("AddCollider");
        GameObject colliderHolder = GameObject.Instantiate(prafabHolder.ColliderHolder, position, Quaternion.identity);
        SphereCollider collider = colliderHolder.AddComponent<SphereCollider>();
        ColliderEventHelper colliderEventHelper = colliderHolder.GetComponent<ColliderEventHelper>();
        colliderEventHelper.timeToLive = effectCollider.duration;

        collider.radius = effectCollider.radius;
        collider.isTrigger = effectCollider.isTrigger;

        if (effectCollider.isTrigger) {
            colliderEventHelper.notifyTriggerEnter += OnTriggerCollider;
        } else {
            colliderEventHelper.notifyCollisionEnter += OnEnterCollider;
        }
    }

    public virtual void OnEnterCollider(GameObject selfObj, Collision collision)
    {
        GameObject hit = collision.gameObject;
        //Debug.Log("handle OnEnterCollider");

        if (hit.tag == creator.GetComponent<Role>().GetEnemyTag())
        {

            ShowBaseEffect(skill.OnTriggerEffect, hit.transform.position);
            Role hitRole = hit.GetComponent<Role>();
            skill.ApplyBuffsToRole(skill.triggeredBuffDefs, hitRole);
            hitRole.ReduceHealth(skill.CalculateValue());
        }
    }

    public virtual void OnTriggerCollider(GameObject selfObj, Collider collider)
    {
        //Debug.Log("handle OnTriggerCollider");
        if (collider.gameObject.tag == creator.GetComponent<Role>().GetEnemyTag())
        {
            ShowBaseEffect(skill.OnTriggerEffect, collider.transform.position);
            Role hitRole = collider.gameObject.GetComponent<Role>();
            skill.ApplyBuffsToRole(skill.triggeredBuffDefs, hitRole);
            Debug.Log("OnTriggerCollider damage " + skill.CalculateValue());
            hitRole.ReduceHealth(skill.CalculateValue());
        }
    }


    void UpdateCollider()
    {
      
        if (colliderChainIndex < skill.colliderChains.Count)
        {
            EffectCollider effectCollider = skill.colliderChains[colliderChainIndex];
           //  Debug.Log("UpdateCollider " + effectCollider.delay + " <= " + pastTime + " - " + (effectCollider.delay <= pastTime));

            if (effectCollider.delay <= pastTime) {
                colliderChainIndex++;
                AddCollider(effectCollider, effectCollider.center == null ? 
                    GeneratePositionByType(effectCollider.position).position : effectCollider.center);
            }
        }
    }
    void UpdateEffect()
    {
      
        if (effectChainIndex < skill.effectChains.Count)
        {
            EffectChain effectChain = skill.effectChains[effectChainIndex];
            if (effectChain.delay <= pastTime)
            {
                effectChainIndex++;
                ShowEffectChain(effectChain, GeneratePositionByType(effectChain.positionType).position);
            }
        }

    }

    public virtual void SkillOnTrail() { }
    public virtual void OnSkillCast() { }
    public virtual void SkillUpdate() { }
    public virtual void OnSkillStop() { }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(position1.position, 5);
    }

    public virtual void InitialSkill() {
        pastTime = 0;
        effectChainIndex = 0;
        colliderChainIndex = 0;
        skill.SkillSetup();

        skill.StartCastSkill();
        OnSkillCast();
        isStarted = true;
    }

    // Start is called before the first frame update
    void Awake()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameController");
        prafabHolder = gameManager.GetComponent<PrafabHolder>();
        commonUtils = gameManager.GetComponentInChildren<CommonUtils>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isStarted) {
            pastTime += Time.deltaTime;

            SkillUpdate();

            if (!skill.hasEffectController)
            {
                UpdateEffect();
            }
            else {
                skill.UpdateEffect();
            }
            if (!skill.hasCollisionController)
            {
                UpdateCollider();
            }
            else {
                skill.UpdateCollider();
            }

            if (pastTime >= skill.duration)
            {
                isStarted = false;
                Destroy(gameObject);
            }
        }

    }

    void OnDestroy()
    {
        //skill.isCasting = false;
        //skill.ResetSkill();
        //Debug.Log("OnDestroy " + skill.skillName + "  " + Time.time);
        if (notifySkillFinish != null) {
            notifySkillFinish();
        }
    }

    public delegate void SkillFinishDelegate();
    public event SkillFinishDelegate notifySkillFinish;
}
