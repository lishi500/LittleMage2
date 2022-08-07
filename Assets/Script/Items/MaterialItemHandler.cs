using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialItemHandler : AbstractItemHandler
{
    public override void CollectItem()
    {
        throw new System.NotImplementedException();
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
        item.type = ItemType.Material;
    }
}
