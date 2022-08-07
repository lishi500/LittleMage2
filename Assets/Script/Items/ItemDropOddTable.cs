using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item/item drop table", order = 1)]
public class ItemDropOddTable : ScriptableObject
{
    public ItemDropOdd[] dropTableOdds = new ItemDropOdd[1];
    public int times = 1;

    [System.Serializable]
    public class ItemDropOdd
    {
        public float chance = 100;
        public ItemDropGroup itemGroup;
    }
}

