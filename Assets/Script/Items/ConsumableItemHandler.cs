using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableItemHandler : AbstractItemHandler
{

    public GameObject buffObject;
    protected BaseBuff baseBuff;
    public RoleType[] exceptedTriggerRole = { RoleType.Player };
    public Role triggerTarget;

    public override void CollectItem()
    {
        GameObject buff = Instantiate(buffObject);
        triggerTarget.AddBuff(buff, triggerTarget);
        Destroy(this);
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        item.type = ItemType.Consumable;
    }
}
