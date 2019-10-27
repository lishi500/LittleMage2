using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUtils : MonoBehaviour
{
    public DamageDef CalculateBulletDamage(float baseDamage, float damageMultiplier, Role from, Role to, DamageType damageType, float criticalAdder)
    {
        float critical = from.attribute.criticalChange + criticalAdder;
        float damageReduce = to.attribute.damageReduce;

        return CalculateDamage(baseDamage, critical, damageType, damageMultiplier, damageReduce);
    }
    public DamageDef CalculateDamage(float baseDamage, Role from, Role to, DamageType damageType) {
        float critical = from.attribute.criticalChange;
        float damageReduce = to.attribute.damageReduce;

        return CalculateDamage(baseDamage, critical, damageType, 1, damageReduce);
    }

    public DamageDef CalculateDamage(float baseDamage, Role to, DamageType damageType) {
        float damageReduce = to.attribute.damageReduce;

        return CalculateDamage(baseDamage, 0, damageType, 1, damageReduce);
    }

    public DamageDef CalculateDamage(float baseDamage, float criticalChance, DamageType damageType, float damageMultiplier, float damageReduce) {
        if (damageType == DamageType.HOLY && damageReduce > 0) {
            damageReduce = 0;
        }
        float damage = baseDamage * damageMultiplier * (1f - damageReduce);
        DamageDef damageDef = new DamageDef(damage, false, damageType);

        return CriticalProcess(damageDef, criticalChance);
    }

    private DamageDef CriticalProcess(DamageDef damageDef, float criticalChance) {
        if (isCritical(criticalChance)) {
            damageDef.isCritical = true;
            damageDef.damage = damageDef.damage * 2;
        }

        return damageDef;
    }
    private bool isCritical(float chance) {
        return chance >= Random.Range(0, 1f);
    }
}
