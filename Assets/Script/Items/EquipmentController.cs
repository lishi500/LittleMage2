using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentController : MonoBehaviour
{
    public EquipmentGears gears;

    Transform playerModel;
    Player player;

    Dictionary<ItemSubType, AttachType> attachMapping;

    public void SwapEquipment(Item equipItem) {
        if (equipItem != null && equipItem.type == ItemType.Equipment) {
            if (GetExistingItem(equipItem.subtype))
            {
                Unequip(equipItem.subtype);
            }
            Equip(equipItem);
        }
    }

    public void Equip(Item item)
    {
        if (item == null || item.type != ItemType.Equipment)
        {
            return;
        }

        EquipItemAttribute(item);
        AssignItem(item.subtype, item);

        Transform attachPoint = FindAttachPoint(playerModel, attachMapping[item.subtype]);
        AttachEquipment(attachPoint, item.itemPrafab);
    }

    public void Unequip(ItemSubType subType)
    {
        Item item = GetExistingItem(subType);
        if (item != null) {
            UnequipItemAttribute(item);
            RemoveExistingMash(item);

            AssignItem(item.subtype, null);
        }
    }

    private void EquipItemAttribute(Item item)
    {
        ItemAttribute itemAttribute = item.attribute;
        if (player != null)
        {
            player.attribute.AddMaxHP(itemAttribute.maxHp);
            player.AddHealth(itemAttribute.maxHp);
            player.attribute.maxHpModifier += itemAttribute.maxHpModifier;


            player.attribute.attack += itemAttribute.attack;
            player.attribute.ChangeAttackModifier(itemAttribute.attackModifier);

            player.attribute.attackSpeed += itemAttribute.attackSpeed;
            player.attribute.ChangeAttackSpeedModifier(itemAttribute.attackSpeed);

            player.attribute.moveSpeed += itemAttribute.moveSpeed;
            player.attribute.ChangeMoveSpeedModifier(itemAttribute.moveSpeedModifier);

            player.attribute.criticalChange += itemAttribute.criticalChange;
            player.attribute.damageReduce += itemAttribute.damageReduce;
            player.attribute.fixShield += itemAttribute.shield;

        }
    }

    private void UnequipItemAttribute(Item item)
    {
        ItemAttribute itemAttribute = item.attribute;
        if (player != null)
        {
            player.attribute.RemoveMaxHP(itemAttribute.maxHp);
            player.SetHealth(Mathf.Min(player.HP, player.attribute.maxHP));
            player.attribute.maxHpModifier -= itemAttribute.maxHpModifier;

            player.attribute.attack -= itemAttribute.attack;
            player.attribute.ChangeAttackModifier(-itemAttribute.attackModifier);

            player.attribute.attackSpeed -= itemAttribute.attackSpeed;
            player.attribute.ChangeAttackSpeedModifier(-itemAttribute.attackSpeed);

            player.attribute.moveSpeed -= itemAttribute.moveSpeed;
            player.attribute.ChangeMoveSpeedModifier(-itemAttribute.moveSpeedModifier);

            player.attribute.criticalChange -= itemAttribute.criticalChange;
            player.attribute.damageReduce -= itemAttribute.damageReduce;
            player.attribute.fixShield -= itemAttribute.shield;
        }
    }

    private void RemoveExistingMash(Item item) {
        string meshName = item.itemName;
        Transform attachPoint = FindAttachPoint(playerModel, attachMapping[item.subtype]);
        Transform existing = SearchHierarchyForBone(attachPoint, meshName);

        if (existing != null) {
            Destroy(existing.gameObject);
        }
    }

    private void AttachEquipment(Transform attachPoint, GameObject equipmentPrafab)
    {
        GameObject replacement = Instantiate(equipmentPrafab, attachPoint.position, Quaternion.identity, attachPoint);
        replacement.transform.localPosition = Vector3.zero;
        replacement.transform.localRotation = Quaternion.identity;
    }

    private Transform FindAttachPoint(Transform roleModel, AttachType attachType)
    {
        string attachName = GetAttachName(attachType);
        return SearchHierarchyForBone(roleModel, attachName);
    }

    private Transform SearchHierarchyForBone(Transform current, string name)
    {
        if (current.name == name)
            return current;
        for (int i = 0; i < current.childCount; ++i)
        {
            Transform found = SearchHierarchyForBone(current.GetChild(i), name);
            if (found != null)
                return found;
        }

        return null;
    }


    private Item GetExistingItem(ItemSubType subType) {
        Item item = null;
        switch (subType) {
            case ItemSubType.Weapon_MainHand:
                item = gears.weapon;
                break;
            case ItemSubType.Weapon_OffHand:
                item = gears.offhandWeapon;
                break;
            case ItemSubType.Head:
                item = gears.helmet;
                break;
            case ItemSubType.Chest:
                item = gears.chest;
                break;
            case ItemSubType.Ring:
                item = gears.ring;
                break;
            case ItemSubType.Necklace:
                item = gears.necklace;
                break;
            default:
                break;
        }

        return item;
    }

    private void AssignItem(ItemSubType position, Item item) {
        switch (position)
        {
            case ItemSubType.Weapon_MainHand:
                gears.weapon = item;
                break;
            case ItemSubType.Weapon_OffHand:
                gears.offhandWeapon = item;
                break;
            case ItemSubType.Head:
                gears.helmet = item;
                break;
            case ItemSubType.Chest:
                gears.chest = item;
                break;
            case ItemSubType.Ring:
                gears.ring = item;
                break;
            case ItemSubType.Necklace:
                gears.necklace = item;
                break;
            default:
                break;
        }
    }

    private string GetAttachName(AttachType attachType)
    {
        string attachName = "";
        switch (attachType)
        {
            case AttachType.Root:
                attachName = "RigPelvis";
                break;
            case AttachType.Head:
                attachName = "+ Head";
                break;
            case AttachType.MainHand:
                attachName = "+ R Hand";
                break;
            case AttachType.OffHand:
                attachName = "+ L Hand";
                break;
            case AttachType.Chest:
                attachName = "+ Back";
                break;
            default:
                attachName = "RigPelvis";
                break;

        }
        return attachName;
    }

    public enum AttachType {
        Root,
        Head,
        MainHand,
        OffHand,
        Chest
    }

    public void InitialEquipmentGears() {
        Equip(gears.weapon);
        Equip(gears.offhandWeapon);
        Equip(gears.helmet);
        Equip(gears.chest);
        Equip(gears.ring);
        Equip(gears.necklace);

    }

    void Start()
    {
        player = Finder.Instance.GetPlayer();
        playerModel = player.transform.GetChild(0);

        attachMapping = new Dictionary<ItemSubType, AttachType>();
        attachMapping.Add(ItemSubType.Head, AttachType.Head);
        attachMapping.Add(ItemSubType.Weapon_MainHand, AttachType.MainHand);
        attachMapping.Add(ItemSubType.Weapon_OffHand, AttachType.OffHand);
        attachMapping.Add(ItemSubType.Chest, AttachType.Chest);

        InitialEquipmentGears();
    }
}
