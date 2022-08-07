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

    public RoleType roleType;

    public List<Skill> skills;
    [SerializeField]
    public List<BaseBuff> buffs;

    public bool isRange;
    public float attackDistance;

    public RoleFlag roleFlag;
    public float maxAttackGapFactor = 1;
    public bool attackIgnoreObstacle;

    public CustomAnimationController animationController;
    public StatusManager statusManager = new StatusManager();

    private SimpleEventHelper eventHelper;
    private TextUtils textUtils;
    protected PrafabHolder prafabHolder;

    public delegate void StatusChangeDelegate(RoleStatus status);
    public event StatusChangeDelegate notifyStatusChange;

    public delegate void DieDelegate();
    public event DieDelegate notifyDied;

    public float AddHealth(float amount) {
        if (isAlive) {
            HP += amount;
            if (HP > attribute.maxHP)
            {
                HP = attribute.maxHP;
            }
            if (amount > 0) {
                GetTextUtils().AddDamageEntry(transform, new DamageDef(amount, false, DamageType.HEAL));
            }
        }
       
        return HP;
    }

    public void SetHealth(float amount) {
        if (isAlive)
        {
            HP = amount;
            if (HP > attribute.maxHP)
            {
                HP = attribute.maxHP;
            }
            if (HP <= 0) {
                Die();
            }
        }
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
                GetTextUtils().AddDamageEntry(transform, new DamageDef(amount, isCritical, type));
                //TextUtils.DamageText(transform, amount, isCritical, type);
                eventHelper.OnGetHit(new DamageDef(amount, isCritical, type));
            }

            if (HP <= 0)
            {
                Die();
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

    public virtual void Die() {
        HP = 0;
        isAlive = false;
        if (notifyDied != null) {
            notifyDied();
        }
        animationController.animationState = AnimationState.DIE;
        eventHelper.notifyDiedEnd += OnDieAnimationEnd;

        ItemDropController itemDropController = GetComponent<ItemDropController>();
        if (itemDropController != null) {
            itemDropController.Drop();
        }

        StartCoroutine(DestroyAfterDie());
    }

    private void OnDieAnimationEnd() {
        GameObject prafabObj = prafabHolder.GetEffect("DieEffect");
        GameObject dieEffect = Instantiate(prafabObj);
        dieEffect.transform.position = this.transform.position;
    }

    private IEnumerator DestroyAfterDie() {
        yield return new WaitForSeconds(0.1f);
        GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
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
            Debug.Log("Add Skill " + skillPrafab.name + "  " + skill.skillName);
            skill.owner = gameObject;
            skills.Add(skill);


            skill.notifySkillReady += SkillReadyTrigger;
            skill.OnSkillAdd();
            skill.ResetCD();

            OnAddSkill(skill);
        }
    }

    protected virtual void OnAddSkill(Skill skill) {
    }
    private void SkillReadyTrigger(Skill skill) {
        eventHelper.OnSkillReadyTrigger(skill);
    }

    public Skill GetMinCDSkill(SkillType[] skillTypes) {
        if (skills != null && skills.Count > 0) {
          

            Skill minCdSkill = skills.Where(skill => skillTypes.Contains(skill.type))
                .OrderBy(vi => vi.CDLeft)
                .ThenBy(vi => vi.skillName)
                .First();
            //.Select((skill, index) => new { skill, index })


            return minCdSkill;
        }
        return null;
    }

    void UpdateAllSkillsCD()
    {
        if (skills.Count > 0) {
            //Debug.Log("UpdateAllSkillsCD " + skills.Count);
            skills.ForEach(skill => skill.UpdateCD());
        }
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

    public float GetAttackRate()
    {
        return 10 / attribute.attackSpeed;
    }

    public virtual string GetBulletName() {
        return "";
    }

    private TextUtils GetTextUtils() {
        if (textUtils == null) {
            GameObject gameManager = GameObject.FindGameObjectWithTag("GameController");
            this.textUtils = gameManager.GetComponentInChildren<TextUtils>();
        }

        return textUtils;
    }
    public virtual void Awake()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameController");
        prafabHolder = gameManager.GetComponent<PrafabHolder>();

        if (attribute == null)
        {
            attribute = new RoleAttribute();
        }
      
        if (buffs == null) {
            buffs = new List<BaseBuff>();
        }

    }

    public virtual void Start() {
        eventHelper = GetComponentInChildren<SimpleEventHelper>();
        animationController = GetComponentInChildren<CustomAnimationController>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        UpdateAllSkillsCD();
    }

    public string[] GetEnemyTags() {
        switch (roleType) {
            case RoleType.Player:
            case RoleType.PlayerMinion:
                return new string[] { TagMapping.Enemy.ToString() };
            case RoleType.EnemyMinion:
            case RoleType.PrimaryEnemy:
                return new string[] { TagMapping.Player.ToString(), TagMapping.Pet.ToString() };
            default:
                return new string[] { TagMapping.Enemy.ToString() };
        }
    }

    public string[] GetAlliesTags() {
        switch (roleType)
        {
            case RoleType.Player:
            case RoleType.PlayerMinion:
                return new string[] { TagMapping.Player.ToString(), TagMapping.Pet.ToString() };
            case RoleType.EnemyMinion:
            case RoleType.PrimaryEnemy:
                return new string[] { TagMapping.Enemy.ToString() };
            default:
                return new string[] { TagMapping.Player.ToString(), TagMapping.Pet.ToString() };
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
