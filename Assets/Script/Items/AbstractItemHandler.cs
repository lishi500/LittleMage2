using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AbstractItemHandler : MonoBehaviour
{
    public Item item;
    public ItemType type {
        get { return item.type; }
    }
    public ItemSubType subtype
    {
        get { return item.subtype; }
    }

    public GameObject OnGroundEffect;
   

    public abstract void CollectItem();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected Player GetPlayer() {
        return Finder.Instance.GetPlayer();
    }

}
