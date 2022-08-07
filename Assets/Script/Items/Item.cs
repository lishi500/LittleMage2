using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Item", menuName = "Item/item", order = 1)]
public class Item : ScriptableObject
{
    public int itemId;
    public string itemName;
    public ItemAttribute attribute;
    public ItemType type;
    public ItemSubType subtype;
    public ItemQuality quality;
    public string description;
    public Sprite icon;

    public int maxStack = 99;
    public int buyPriceCoin;
    //public int buyPriceSoul;
    //public int buyPriceDiamond;
    public int salePriceCoin;
    //public int salePriceSoul;
    //public int salePriceDiamond;

    // public int level = 1;
    // public int maxLevel;
    // public UpgradeCost upgradeCost;
    // public ItemAttribute upgradeAttribute;

    public GameObject itemDropPrafab;
    public GameObject itemPrafab;

    public void IsLala() {
        Debug.Log("item subtype " + subtype);
    }
}
