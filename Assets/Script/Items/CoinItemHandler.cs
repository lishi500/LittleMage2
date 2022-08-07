using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinItemHandler : AbstractItemHandler
{
    public override void CollectItem()
    {
        switch (subtype) {
            case ItemSubType.Coin:

                break;
            case ItemSubType.Experience:
                GetPlayer().GainExperience((int)item.attribute.amount1);
                break;
            case ItemSubType.Soul:
                break;
            case ItemSubType.Diamond:
                break;
            default:
                break;
        }
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
        item.type = ItemType.Coin;
    }
}
