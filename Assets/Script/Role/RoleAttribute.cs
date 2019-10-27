using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoleAttribute
{
    public int level;
    public float maxHP;

    [SerializeField]
    private float _attack;
    public float attack {
        get { return (_attack + attackAdder) * attackModifier; }
        set { _attack = value; }
    } // also magic

    private float _attackAdder = 0;
    public float attackAdder
    {
        get { return _attackAdder; }
        set { _attackAdder = value; }
    } // also magic

    private float _attackModifier = 1;
    public float attackModifier
    {
        get { return _attackModifier; }
        set { _attackModifier = value; }
    }
    public void ChangeAttackAdder(float attackChange)
    {
        attackAdder = attackAdder + attackChange;
    }
    public void ChangeAttackModifier(float percent)
    {
        attackModifier = attackModifier + percent;
    }

    [SerializeField]
    private float _criticalChange = 0;
    public float criticalChange {
        get {
            return _criticalChange + criticalAdder;
        }
        set {
            _criticalChange = value;
        }
    }
    public float criticalAdder = 0;


    //[SerializeField]
    //private float _defence;
    //public float defence
    //{
    //    get { return _defence; }
    //    set
    //    {
    //        _defence = value;
    //    }
    //}

    // percentage of damage reduce
    [SerializeField]
    private float _damageReduce;
    public float damageReduce
    {
        get { return _damageReduce; }
        set
        {
            _damageReduce = value;
        }
    }

    //public float magic;
    [SerializeField]
    private float _attackSpeed;
    public float attackSpeed {
        get { return _attackSpeed * _attackSpeedModifier; }
        set { _attackSpeed = value;  }
    }
    [SerializeField]
    private float _attackSpeedModifier = 1;
    public float attackSpeedModifier {
        get { return _attackSpeedModifier; }
        set {
            _attackSpeedModifier = value;
            TriggerAttackSpeedChange(attackSpeed);
        }
    }
    public void ChangeAttackSpeedModifier(float percent) {
        attackSpeedModifier = attackSpeedModifier + percent;
    }

    [SerializeField]
    private float _moveSpeed;
    public float moveSpeed
    {
        get { return _moveSpeed * _moveSpeedModifier;  }
        set { _moveSpeed = value; }
    }
    [SerializeField]
    private float _moveSpeedModifier = 1;
    public float moveSpeedModifier
    {
        get { return _moveSpeedModifier; }
        set {
            _moveSpeedModifier = value;
            TriggerMoveSpeedChange(moveSpeed);
        }
    }

    public void ChangeMoveSpeedModifier(float percent)
    {
        moveSpeedModifier += percent;
    }

    public float shield;
    public int exprience;
    public float toughness = 0.75f; // 2 is hardcore 



    public delegate void AttackSpeedChangeDelegate(float attackSpeed);
    public event AttackSpeedChangeDelegate notifyAttackSpeedChange;
    private void TriggerAttackSpeedChange(float attackSpeed) {
        Debug.Log("TriggerAttackSpeedChange " + attackSpeed);
        if (notifyAttackSpeedChange != null) {
            notifyAttackSpeedChange(attackSpeed);
        }
    }

    public delegate void MoveSpeedChangeDelegate(float moveSpeed);
    public event MoveSpeedChangeDelegate notifyMoveSpeedChange;
    private void TriggerMoveSpeedChange(float moveSpeed)
    {
        Debug.Log("TriggerMoveSpeedChange " + moveSpeed);

        if (notifyMoveSpeedChange != null)
        {
            notifyMoveSpeedChange(moveSpeed);
        }
    }
}

