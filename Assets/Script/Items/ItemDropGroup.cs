using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item/item drop group", order = 1)]
public class ItemDropGroup : ScriptableObject
{
    public Item[] items;
}
