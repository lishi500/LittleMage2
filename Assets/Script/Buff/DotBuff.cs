using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotBuff : BaseBuff
{

    public override void OnBuffEvaluated()
    {
        int expectedCount = CalculateExpectedTirggerCount();
        //Debug.Log("OnBuffEvaluated " + expectedCount + "  " + tirggeredCount);
        if (expectedCount > tirggeredCount) {
            //Debug.Log("Trigger " + expectedCount);
            TriggerDot();
            ShowEffect(OnTriggerEffect);
            tirggeredCount++;
        }
    }

  
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

    // Update is called once per frame

}
