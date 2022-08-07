using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item/equipment gears", order = 1)]
public class EquipmentGears : ScriptableObject
{
    public Item weapon;
    public Item offhandWeapon;
    public Item helmet;
    public Item chest;
    public Item ring;
    public Item necklace;
}
