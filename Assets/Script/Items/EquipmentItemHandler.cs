using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentItemHandler : AbstractItemHandler
{
    public ItemAttribute itemAttribute {
        get { return item.attribute; }
    }
    public override void CollectItem()
    {
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        item.type = ItemType.Equipment;
    }
}
