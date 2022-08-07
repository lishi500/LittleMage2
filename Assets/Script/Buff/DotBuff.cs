using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotBuff : BaseBuff
{
    private void TriggerDot() {
        float damage = CalculatValue();
       
        if (holder != null) {
            holder.ReduceHealth(damage);
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        type = BuffType.DOT;
    }

    public override void OnBuffApply()
    {
    }

    public override void OnBuffRemove()
    {
    }

    public override BuffEvaluatorResult OnBuffEvaluated(BuffEvaluatorResult evaluatorResult)
    {
        throw new NotImplementedException();
    }

    public override void OnBuffTrigger()
    {
        int expectedCount = CalculateExpectedTirggerCount();
        //Debug.Log("OnBuffEvaluated " + expectedCount + "  " + tirggeredCount);
        if (expectedCount > tirggeredCount)
        {
            //Debug.Log("Trigger " + expectedCount);
            TriggerDot();
            ShowEffect(OnTriggerEffect);
            tirggeredCount++;
        }
    }

    // Update is called once per frame

}
