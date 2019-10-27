using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

abstract public class Role : MonoBehaviour
{
    // Start is called before the first frame update
    int id;
    [Header("Attribute")]
    public RoleAttribute attribute;
    public float HP;
    [HideInInspector]
    private bool _isAlive = true; 
    public bool isAlive {
        get { return _isAlive; }
        set {
            _isAlive = value;
            // notifiy dead
        }
    }
    //[HideInInspector]
    public bool isChanneling;

    public AlignmentType alignment;

    public List<Skill> skills;
    [SerializeField]
    public List<BaseBuff> buffs;

    public bool isRange;
    public float attackDistance;

    public StatusManager statusManager = new StatusManager();
    private SimpleEventHelper eventHelper;

    public delegate void StatusChangeDelegate(RoleStatus status);
    public event StatusChangeDelegate notifyStatusChange;

    public delegate void DieDelegate(float isDied);
    public event DieDelegate notifyDied;

    public float AddHealth(float amount) {
        if (isAlive) {
            HP += amount;
            if (HP > attribute.maxHP)
            {
                HP = attribute.maxHP;
            }
        }
       
        return HP;
    }

    public float ReduceHealth(DamageDef damageDef) {
        return ReduceHealth(damageDef.damage, damageDef.isCritical, damageDef.type);
    }
    public float ReduceHealth(float amount, bool isCritical = false, DamageType type = DamageType.NORMAL)
    {
        if (isAlive) {
            if (attribute.shield > 0)
            {
                float amountLeft = Mathf.Max(amount - attribute.shield, 0);
                ReduceShield(amount);

                amount = amountLeft;
            }

            HP -= amount;
            if (amount > 0) {
                TextUtils.DamageText(transform, amount, isCritical, type);
                eventHelper.OnGetHit(new DamageDef(amount, isCritical, type));
            }

            if (HP <= 0)
            {
                HP = 0;
                isAlive = false;
            }
        }
       
        return HP;
    }

    private void ReduceShield(float amount)
    {
        attribute.shield -= amount;
        if (attribute.shield < 0) {
            attribute.shield = 0;
        }
    }

    public virtual bool CanMove() {
        return statusManager.CanMove() && isAlive;
    }

    public virtual bool CanCast() {
        return statusManager.CanCast() && isAlive;
    }

    public bool CanAction() {
        return statusManager.CanAction() && isAlive;
    }


    public void AddSkill(GameObject skillPrafab) {
        Skill skill = skillPrafab.GetComponent<Skill>();
        if (skill != null) {
            //Debug.Log("Add Skill " + skillPrafab.name + "  " + skill.skillName);
            skill.owner = gameObject;
            skills.Add(skill);
            skill.OnSkillAd();
            skill.ResetCD();
        }
    }

    public Skill GetMinCDSkill(SkillType[] skillTypes) {
        if (skills != null && skills.Count > 0) {
            Skill minCdSkill = skills.Where(skill => skillTypes.Contains(skill.type))
                .Select((skill, index) => new { skill, index })
                .OrderBy(vi => vi.skill.CDLeft)
                .First().skill;

            return minCdSkill;
        }
        return null;
    }

    void UpdateCD(Skill skill)
    {
        if (skill.CDLeft > 0)
        {
            skill.CDLeft -= Time.deltaTime;
            //Debug.Log("UpdateCD " + skill.skillName + " " + skill.CDLeft);
        }

        if (skill.CDLeft <= 0)
        {
            skill.CDLeft = 0;
            if (eventHelper != null) {
                eventHelper.OnSkillReadyTrigger(skill);
            }
        }
    }
    void UpdateAllSkillsCD()
    {
        skills.ForEach(skill => UpdateCD(skill));
    }

    public void AddBuff(GameObject buffObj, Role caster)
    {
        bool exist = false;
        BaseBuff buff = buffObj.GetComponent<BaseBuff>();
        buff.caster = caster;

        foreach (BaseBuff baseBuff in buffs)
        {
            if (baseBuff.buffName == buff.buffName && !buff.canDuplicate)
            {
                exist = true;
                baseBuff.duration = buff.duration;
                baseBuff.value = buff.value;
                baseBuff.factor = buff.factor;
                baseBuff.RenewBuff();

                Destroy(buffObj);
                break;
            }
        }

        if (!exist)
        {
            buffObj.transform.SetParent(this.transform);
            buffObj.transform.position = this.transform.position;
            buff.notifyBuffRemoved += OnBuffDestory;
            buffs.Add(buff);
        }
    }

    private void OnBuffDestory(GameObject buffObj) {
        BaseBuff buff = buffObj.GetComponent<BaseBuff>();
        buffs.Remove(buff);
    }

    public Skill GetSkillByName(string skillName) {
        foreach (Skill nextSkill in skills) {
            if (nextSkill.skillName == skillName) {
                return nextSkill;
            }
        }

        return null;
    }
    public virtual void Awake()
    {
        if (attribute == null)
        {
            attribute = new RoleAttribute();
        }
      
        if (buffs == null) {
            buffs = new List<BaseBuff>();
        }

        eventHelper = GetComponentInChildren<SimpleEventHelper>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        UpdateAllSkillsCD();
    }

    public string GetEnemyTag() {
        switch (alignment) {
            case AlignmentType.Player:
            case AlignmentType.PlayerAlign:
                return "Enemy";
            case AlignmentType.Enemy:
                return "Player";
            default:
                return "Enemy";
        }
    }

    public virtual Transform GetShootPoint(ShootPointPosition pointType) {
        Transform point;
        switch (pointType) {
            case ShootPointPosition.MID:
                point = transform.Find("ShootPoint");
                break;
            case ShootPointPosition.RECEIVE:
                point = transform.Find("ReceivePoint");
                break;
            default:
                point = transform;
                break;
        }
        if (point == null) {
            point = transform;
        }

        return point;
    }
}
