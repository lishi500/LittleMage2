using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemDropController : MonoBehaviour
{
    public float[] dropChances;
    public ItemDropOddTable[] dropTables;
    private int MAX_RETRY = 100;
    public void Drop() {

        if (CanDrop()) {
            List<Item> items = Loot();
            //Debug.Log("Drop Items " + items.Count);
            DropItems(items);
        }
    }

    public void Drop(int number) {
        if (CanDrop()) {
            int retry = 0;
            List<Item> items = new List<Item>();
            while (items.Count < number && retry < MAX_RETRY)
            {
                List<Item> lootItems = Loot();
                if (lootItems.Count > 0)
                {
                    retry = 0;
                    items.Concat(lootItems);
                }
                else
                {
                    retry++;
                }
            }

            DropItems(items);
        }
       
    }

    private List<Item> Loot() {
        List<Item> dropItems = new List<Item>();
        int iterator = Mathf.Min(dropChances.Length, dropTables.Length);

        for (int i = 0; i < iterator; i++)
        {
            if (Random.Range(0, 100f) < dropChances[i])
            {
                List<Item> items = LootInTable(dropTables[i]);
                if (items.Count > 0) {
                    dropItems.AddRange(items);
                }
            }
        }

        return dropItems;
    }

    private List<Item> LootInTable(ItemDropOddTable dropTable) {
        List<Item> dropItems = new List<Item>();
        for (int i = 0; i < dropTable.times; i++) {
            Item item = LootOneInTable(dropTable);
            if (item != null) {
                dropItems.Add(item);
            }
        }

        return dropItems;
    }

    private Item LootOneInTable(ItemDropOddTable dropTable) {
        float totalChance = 0;
        foreach (ItemDropOddTable.ItemDropOdd odd in dropTable.dropTableOdds) {
            totalChance += odd.chance;
        }

        float lootNumber = Random.Range(0, totalChance);
        float accumulateChange = 0;
        ItemDropGroup selectedGroup = null;

        for (int i = 0; i < dropTable.dropTableOdds.Length; i++)
        {
            accumulateChange += dropTable.dropTableOdds[i].chance;
            if (lootNumber <= accumulateChange)
            {
                selectedGroup = dropTable.dropTableOdds[i].itemGroup;
                break;
            }
        }

        return LootInGroup(selectedGroup);
    }

    private Item LootInGroup(ItemDropGroup group) {
        float lootNumber = Random.Range(0, 99.99f);
        
        if (group.items != null && group.items.Length > 0) {
            float slice = 100 / group.items.Length;
            int index = Mathf.FloorToInt(lootNumber / slice);
            return group.items[index];
        }

        return null;
    }

    private void DropItems(List<Item> items) {
        foreach (Item item in items) {
            DropItem(item);
        }
    }

    private void DropItem(Item item) {
        //Debug.Log("Drop " + item.itemName);
        if (item != null && item.itemDropPrafab != null) {
            GameObject itemObj = GameObject.Instantiate(item.itemDropPrafab);
            itemObj.transform.position = transform.position;
        }
    }


    private bool CanDrop() {
        if (dropChances != null && dropTables != null) {
            if (dropChances.Length != dropTables.Length)
            {
                Role role = GetComponent<Role>();
                string message = role != null ? role.name : "";
                Debug.LogError("drop table and drop change does not match: " + message);
            }
            if (dropChances.Length != 0 && dropTables.Length != 0) {
                return true;
            }
        }

        return false;
    }

}
