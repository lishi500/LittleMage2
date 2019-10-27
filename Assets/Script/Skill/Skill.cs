using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Skill : MonoBehaviour
{
    public string skillName; 
    public float CD;
    public float duration;
    public float baseValue;
    public float factor;
    public DamageType damageType;

    public bool loopCast;
    public bool isChannelSkill;
    public SkillType type;
    public ShootPointPosition shootPointPosition = ShootPointPosition.MID;

    public bool hasEffectController;
    public bool hasCollisionController;
    public bool needTarget = true;

    [HideInInspector]
    public float CDLeft = 0;
    [HideInInspector]
    public GameObject owner;
    [HideInInspector]
    public GameObject skillControllerObj;


    public List<EffectChain> effectChains;
    public List<EffectCollider> colliderChains;
    public List<SkillAttachedBuff> triggeredBuffDefs;
    public List<SkillAttachedBuff> onApplyBuffDefs;

    public BaseEffect OnCastEffect; // position caster
    public BaseEffect OnTriggerEffect;

    public abstract void OnSkillAd();
    public abstract void OnSkillCast();
    public abstract void UpdateCollider();
    public abstract void UpdateEffect();

    public virtual float CalculateValue() {
        float attack = owner.GetComponent<Role>().attribute.attack;
        return baseValue + (attack * factor);
    }

    public virtual void ApplyBuffsToRole(List<SkillAttachedBuff> buffDefs, Role role) {
        foreach (SkillAttachedBuff buffDef in buffDefs) {
            GameObject buffObj = Instantiate(buffDef.buffObj);
            if (buffDef.overrideExisting) {
                BaseBuff baseBuff = buffObj.GetComponent<BaseBuff>();
                baseBuff.duration = buffDef.duration;
                baseBuff.frequency = buffDef.frequency;
                baseBuff.value = buffDef.value;
                baseBuff.factor = buffDef.factor;
            }
            role.AddBuff(buffObj, owner.GetComponent<Role>());
        }
    }

    public void StartCastSkill() {
        if (CDLeft <= 0) {
            CDLeft = CD;
        }
        OnSkillCast();
    }

    public void StartCD() {
        if (CDLeft <= 0)
        {
            CDLeft = CD;
        }
    }
    public void ResetCD() {
        CDLeft = 0;
    }

    public bool IsReady() {
        return CDLeft <= 0;
    }

    public abstract void SkillSetup();
  
    public virtual void Start()
    {
        CDLeft = 0;
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameController");

    }
    public virtual void Update()
    {
        if (hasEffectController)
        {
            UpdateEffect();
        }
        if (hasCollisionController)
        {
            UpdateCollider();
        }
    }  
}
