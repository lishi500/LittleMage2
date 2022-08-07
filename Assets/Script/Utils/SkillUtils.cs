using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillUtils : Singleton<SkillUtils>
{
    DamageType[] defensiveType = new DamageType[] { DamageType.NONE, DamageType.HEAL, DamageType.SHIELD };
    public string[] GetSkillTargetsTags(Skill skill)
    {
        if (defensiveType.Contains(skill.damageType))
        {
            return skill.ownerRole.GetAlliesTags();
        }
        else {
            return skill.ownerRole.GetEnemyTags();
        }
    }

    public Vector3 GetPositionWithDistanceAndAngle(Transform from, float distance, float angle)
    {
        Vector3 newPos = from.position + Quaternion.AngleAxis(angle, Vector3.up) * from.forward * distance;
        return newPos;
    }
}
