using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeBuff : BaseBuff
{
    [Header("AttributeBuff")]
    public AttributeType attributeType;
    private float originalValue;
    public override void OnBuffApply()
    {
        Debug.Log("OnBuffApply " + buffName);

        switch (attributeType) {
            case AttributeType.ATTACK_ADDER:
                holder.attribute.attackAdder += value;
                break;
            case AttributeType.ATTACK_MODIFIER:
                holder.attribute.ChangeAttackModifier(value);
                break;

            case AttributeType.ATTACK_SPEED_MODIFIER:
                holder.attribute.ChangeAttackSpeedModifier(value);
                break;

            case AttributeType.MOVE_SPEED_MODIFIER:
                holder.attribute.ChangeMoveSpeedModifier(value);
                break;
            case AttributeType.CRITICAL_CHANGE:
                holder.attribute.criticalAdder += value;
                break;

            case AttributeType.DAMAGE_REDUCE:
                holder.attribute.damageReduce += value;
                break;
            case AttributeType.MAX_HP:
                //originalValue = holder.attribute.maxHP;
                holder.attribute.AddMaxHP(value);
                holder.AddHealth(value);
                break;
            case AttributeType.MAX_HP_MODIFIER:
                //originalValue = holder.attribute.maxHP;
                float addValue = holder.attribute.maxHP * value;
                holder.attribute.maxHpModifier += value;
                holder.AddHealth(addValue);
                break;
            case AttributeType.SHIELD:
                originalValue = holder.attribute.shield;
                holder.attribute.shield += value;
                break;
            default:
                break;
        }
    }

    public override void OnBuffRemove()
    {
        switch (attributeType)
        {
            case AttributeType.ATTACK_ADDER:
                holder.attribute.attackAdder -= value;
                break;
            case AttributeType.ATTACK_MODIFIER:
                holder.attribute.ChangeAttackModifier(-value);
                break;
            case AttributeType.ATTACK_SPEED_MODIFIER:
                holder.attribute.ChangeAttackSpeedModifier(-value);
                break;
            case AttributeType.MOVE_SPEED_MODIFIER:
                holder.attribute.ChangeMoveSpeedModifier(-value);
                break;
            case AttributeType.CRITICAL_CHANGE:
                holder.attribute.criticalAdder -= value;
                break;
          
            case AttributeType.DAMAGE_REDUCE:
                holder.attribute.damageReduce -= value;
                break;
            case AttributeType.MAX_HP:
                holder.attribute.RemoveMaxHP(value);
                if (holder.HP > holder.attribute.maxHP) {
                    holder.HP = holder.attribute.maxHP;
                }
                break;
            case AttributeType.MAX_HP_MODIFIER:
                holder.attribute.maxHpModifier -= value;
                if (holder.HP > holder.attribute.maxHP)
                {
                    holder.HP = holder.attribute.maxHP;
                }
                break;
            //case AttributeType.SHIELD:
            //    originalValue = holder.attribute.shield;
            //    holder.attribute.shield += value;
            //    break;
            default:
                break;
        }
    }

    public override BuffEvaluatorResult OnBuffEvaluated(BuffEvaluatorResult evaluatorResult)
    {
        return evaluatorResult;
    }

    public override void OnBuffTrigger()
    {
    }

}
