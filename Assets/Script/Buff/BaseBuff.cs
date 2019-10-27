using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBuff : MonoBehaviour
{
    public string buffName;
    public float duration;
    public float frequency; // gap
    public BuffType type;
    public List<ReactEventType> reactTypes;

    public Role caster;
    public Role holder;

    public float value;
    public float factor;

    public bool isDeBuff;
    public bool isForever;
    public bool canDuplicate;
    public string attachedSkillName;

    public BaseEffect OnApplyEffect;
    public BaseEffect OnTriggerEffect;
    public BaseEffect LivingEffect;
    public BaseEffect[] OtherEffects;


    private float timePasted;
    [HideInInspector]
    public int tirggeredCount;
    [HideInInspector]
    SimpleEventHelper eventHelper;
    [HideInInspector]
    public PrafabHolder prafabHolder;
    [HideInInspector]
    public delegate void OnBuffRemoveDelegate(GameObject self);
    [HideInInspector]
    public event OnBuffRemoveDelegate notifyBuffRemoved;


    public abstract void OnBuffEvaluated();
    public abstract void OnBuffApply();
    public abstract void OnBuffRemove();

    public virtual void OnAttack(string state) {
        Debug.Log("OnAttack BaseBuff");
    }
    public virtual void OnCastSkill(string state)
    {

    }

    public virtual void OnSkillReady(Skill skill)
    {

    }
    public virtual void OnGetHit(DamageDef damageDef)
    {

    }
    public virtual void OnEnemyEnterOuterZone(string state)
    {

    }
    public virtual void OnEnemyCollision(string state)
    {

    }
    public virtual void OnDie(string state)
    {

    }
    public virtual float CalculatValue() {
        return value + caster.attribute.attack * factor;
    }
 
    public void RenewBuff() {
        if (timePasted > 0) {
            if (frequency > 0)
            {
                timePasted = timePasted % frequency;
                tirggeredCount = 1;
            }
            else {
                timePasted = 0;
                tirggeredCount = 0;
            }
        }
    }
    // Start is called before the first frame update
    void RegisterEventListener() {
        if (eventHelper != null)
        {
            foreach (ReactEventType reactEventType in reactTypes)
            {
                switch (reactEventType)
                {
                    case ReactEventType.ATTACK:
                        eventHelper.notifyRangeAttack += OnAttack;
                        eventHelper.notifyMeleeAttack += OnAttack;
                        break;
                    case ReactEventType.CAST_SKILL:
                        eventHelper.notifyCastSpell += OnCastSkill;
                        break;
                    case ReactEventType.SKILL_READY:
                        eventHelper.notifySkillReady += OnSkillReady;
                        break;
                    case ReactEventType.GET_HIT:
                        eventHelper.notifyGetHit += OnGetHit;
                        break;
                    case ReactEventType.ENEMY_ENTER_OUT_ZONE:
                        eventHelper.notifyEnemyEnterOuterZone += OnEnemyEnterOuterZone;
                        break;
                    case ReactEventType.ENEMY_COLLISION:
                        eventHelper.notifyEnemyCollisione += OnEnemyCollision;
                        break;
                    case ReactEventType.DIE:
                        eventHelper.notifyDied += OnDie;
                        break;
                    default:
                        break;
                }
            }
        } 
    }

    public void ShowEffect(BaseEffect baseEffect, bool positionOnly = false)
    {
        GameObject effectObj = null;

        if (baseEffect.effect != null) {
            if (positionOnly)
            {
                effectObj = GameObject.Instantiate(baseEffect.effect, transform.position, Quaternion.identity);
            }
            else {
                effectObj = Instantiate(baseEffect.effect);
                effectObj.transform.SetParent(gameObject.transform);
                effectObj.transform.localPosition = Vector3.zero;
            }
           

            if (baseEffect.duration > 0)
            {
                effectObj.AddComponent<AutoDestroy>().timeToLive = baseEffect.duration;
            }

            ParticleSystem ps = effectObj.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
        }
    }


    public virtual void Start()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameController");
        prafabHolder = gameManager.GetComponent<PrafabHolder>();
        holder = GetComponentInParent<Role>();
        PlayerController playerController = GetComponentInParent<PlayerController>();
        BaseAI baseAI = GetComponentInParent<BaseAI>();

        if (baseAI != null) {
            eventHelper = baseAI.eventHelper;
        }
        if (playerController != null)
        {
            eventHelper = playerController.eventHelper;
        }
       
        RegisterEventListener();

        OnBuffApply();
        ShowEffect(OnApplyEffect);
        ShowEffect(LivingEffect);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTime();
        OnBuffEvaluated();
    }

    public int CalculateExpectedTirggerCount()
    {
        if (frequency >= 0) {
            return (int) Math.Ceiling(timePasted / frequency);
        }
        return 0;
    }

    private void UpdateTime()
    {
        if (!isForever)
        {
            timePasted += Time.deltaTime;

            if (timePasted > duration)
            {
                if (notifyBuffRemoved != null)
                {
                    notifyBuffRemoved(gameObject);
                }

                Destroy(gameObject);
            }
        }

    }

    public CustomAnimationController GetAnimationController() {
       return gameObject.transform.parent.GetComponentInChildren<CustomAnimationController>();
    }

    public bool IsPlayer() {
        return GetComponentInParent<Player>() != null;
    }

    public void OnDestroy()
    {
        OnBuffRemove();
    }
}
