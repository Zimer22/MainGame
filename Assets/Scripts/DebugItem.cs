using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[ExecuteInEditMode]
public class DebugItem : Item
{
    NetworkManager networkManager;
    void Awake()
    {
        InitializeItemOffline();
        interactions.Add(0, "autopickup");
        beginInteract += Pickup;
    }
    void Pickup(ulong a,int b)
    {
        switch (b)
        {
            case 0:
                NetworkManager.Singleton.SpawnManager.SpawnedObjects[a].GetComponent<Player>().Inventories[0].AddItemToInventoryRpc(ItemManager.Instance.itemNetIds[Id.Value]);
                break;
        }
    }
}