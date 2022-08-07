using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealBuff : BaseBuff
{
    public override void OnBuffApply()
    {
    }

    public override void OnBuffTrigger()
    {
        int expectedCount = CalculateExpectedTirggerCount();
        Debug.Log("OnBuffEvaluated " + buffName + ": " + expectedCount + "  " + tirggeredCount);
        if (expectedCount > tirggeredCount)
        {
            Debug.Log("Trigger " + expectedCount);
            TriggerHot();
            ShowEffect(OnTriggerEffect);
            tirggeredCount++;
        }
    }

    public override BuffEvaluatorResult OnBuffEvaluated(BuffEvaluatorResult evaluatorResult)
    {
        return evaluatorResult;
    }

    private void TriggerHot()
    {
        float heal = CalculatValue();

        if (holder != null)
        {
            holder.AddHealth(heal);
        }
    }

    public override void OnBuffRemove()
    {
    }

    // Start is called before the first frame update
    void Awake()
    {
        type = BuffType.HOT;
    }

}
