using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffEvaluator : Singleton<BuffEvaluator>
{
    void Evaluate(Role role, ReactEventType eventType) {
        if (role != null) {
            List<BaseBuff> evalutableBuffs = FilterBuffByReactType(role.buffs, eventType);
            BuffEvaluatorResult evaluatorResult = new BuffEvaluatorResult();
            evaluatorResult.Init();

            foreach (BaseBuff baseBuff in evalutableBuffs) {

            }
        }
    }

    private List<BaseBuff> FilterBuffByReactType(List<BaseBuff> buffs, ReactEventType eventType) {
        return buffs.Where(buff =>
        {
            return buff.reactTypes != null && buff.reactTypes.Contains(eventType);
        }).OrderBy(buff => {
            return buff.prioirty;
        })
        .ToList<BaseBuff>();
    }

}
