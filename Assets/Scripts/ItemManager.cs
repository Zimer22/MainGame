using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static UnityEditor.Progress;
public class ItemManager : Singleton<ItemManager> //todo reorganize managers. item and object manager should not be separate as they both have redundancies, and tbh this whole things a mess
{
    public Dictionary<ulong, Item> Items = new();
    public Dictionary<ulong, Inventory> Inventories = new();
    public Dictionary<ulong, InventoryGrid> InventoryGrids = new(); //networkvariablekey
    public Dictionary<int, ulong> inventoryNetIds = new(); //networkvariableid key ,networkobjectid value
    public Dictionary<int, ulong> itemNetIds = new();
    public int AddItem(Item item)
    {
        int itemId = itemNetIds.Count + 1; 
        Items.Add(item.NetworkObjectId, item);
        itemNetIds.Add(itemId, item.NetworkObjectId);
        ObjectManager.Instance.AddItem(item);
        return itemId;
    }
    public void AddExistingItem(int itemId,Item item)
    {
        Items.TryAdd(item.NetworkObjectId, item);
        itemNetIds.TryAdd(itemId, item.NetworkObjectId);
        ObjectManager.Instance.AddItem(item);
    }
    public Item GetItem(int itemId)
    {
        return Items[itemNetIds[itemId]];
    }
    public Inventory GetInventory(int invId)
    {
        return Inventories[inventoryNetIds[invId]];
    }
    public InventoryGrid GetGrid(int invId)
    {
        return InventoryGrids[inventoryNetIds[invId]];
    }
    public int AddInventory(Inventory inventory)
    {
        int invId = inventoryNetIds.Count + 1;
        Inventories.Add(inventory.NetworkObjectId, inventory);
        inventoryNetIds.Add(invId, inventory.NetworkObjectId);
        return invId;
    }
    public void AddExistingInventory(int invid, Inventory inv)
    {
        Inventories.TryAdd(inv.NetworkObjectId, inv);
        inventoryNetIds.TryAdd(invid, inv.NetworkObjectId);
    }
    public void AddGrid(InventoryGrid grid) 
    {
        InventoryGrids.Add(inventoryNetIds[grid.Inventory.Id.Value], grid);
    }
}
