using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 *  ATTACK,
   GET_HIT,
   CAST_SKILL,
   DIE,
   ENEMY_ENTER_OUT_ZONE,
   ENEMY_COLLISION
 */
public class SimpleEventHelper : MonoBehaviour
{
    public delegate void MeleeAttackDelegate(string state);
    public event MeleeAttackDelegate notifyMeleeAttack;

    public delegate void RangeAttackDelegate(string state);
    public event RangeAttackDelegate notifyRangeAttack;

    public delegate void CastSpellDelegate(string state);
    public event CastSpellDelegate notifyCastSpell;

    public delegate void SkillReadyDelegate(Skill skill);
    public event SkillReadyDelegate notifySkillReady;

    public delegate void GetHiDelegate(DamageDef def);
    public event GetHiDelegate notifyGetHit;

    public delegate void GetHitEndDelegate();
    public event GetHitEndDelegate notifyGetHitEnd;
  
    public delegate void DieDelegate(string state);
    public event DieDelegate notifyDied;

    public delegate void DieAnimationEndDelegate();
    public event DieAnimationEndDelegate notifyDiedEnd;

    public delegate void EnemyEnterOuterZoneDelegate(string state);
    public event EnemyEnterOuterZoneDelegate notifyEnemyEnterOuterZone;

    public delegate void EnemyCollisioneDelegate(string state);
    public event EnemyCollisioneDelegate notifyEnemyCollisione;

    public delegate void FirstPhaseCompleteDelegate(AnimationState animationState);
    public event FirstPhaseCompleteDelegate notifyFirstPhaseComplete;

    public delegate void SkillAnimationEndDelegate();
    public event SkillAnimationEndDelegate notifySkillAnimationEnd;

    public virtual void OnMeleeAttackTrigger(string state)
    {
        if (notifyMeleeAttack != null)
        {
            notifyMeleeAttack(state);
        }
    }

    public void OnRangeAttackTrigger(string state)
    {
        if (notifyRangeAttack != null)
        {
            notifyRangeAttack(state);
        }
    }

    public void OnCastSpellTrigger(string state)
    {
        if (notifyCastSpell != null)
        {
            notifyCastSpell(state);
        }
    }

    public void OnSkillReadyTrigger(Skill skill) {
        if (notifySkillReady != null) {
            notifySkillReady(skill);
        }
    }

    public void OnGetHit(DamageDef def) {
        if (notifyGetHit != null) {
            notifyGetHit(def);
        }
    }
    public void OnGetHitEnd()
    {
        //Debug.Log("OnGetHitEnd triggered");
        if (notifyGetHitEnd != null)
        {
            notifyGetHitEnd();
        }
    }

    public void OnDieEnd()
    {
        if (notifyDiedEnd != null) { 
            notifyDiedEnd();
        }
    }


    public void OnFirstPhaseComplete(string stateName) {
        //Debug.Log("OnFirstPhaseComplete " + stateName);
        if (notifyFirstPhaseComplete != null) {
            AnimationState animationState = (AnimationState)System.Enum.Parse(typeof(AnimationState), stateName);
            notifyFirstPhaseComplete(animationState);
        }
    }

    public void OnSkillAnimationEnd() {
        if (notifySkillAnimationEnd != null) {
            notifySkillAnimationEnd();
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
