using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;
public class ObjectManager : Singleton<ObjectManager> //todo reorganize managers. item and object manager should not be separate as they both have redundancies
{
    public Dictionary<ulong, Interactable> interClasses = new();
    public Dictionary<ulong, GameObject> interObjects = new(); //might be unnecessary
    public Dictionary<int, ulong> interactableObjNetIds = new();
    public Dictionary<int, ulong> interactableClassNetIds = new();
    public void AddInteractable(Interactable item)
    {
        ulong netId = item.NetworkObjectId;
        interClasses.Add(netId, item);
        interObjects.Add(netId, item.gameObject);
        interactableObjNetIds.Add(item.gameObject.GetInstanceID(), netId);
        interactableClassNetIds.Add(item.GetInstanceID(), netId); 
    }   
    public void AddItem(Item item)
    {
        ulong netId = item.NetworkObjectId;
        interactableObjNetIds.Add(item.gameObject.GetInstanceID(), netId);
        interactableClassNetIds.Add(item.GetInstanceID(), netId);
    }
    public bool IsInteractableOrItem(ulong id) //true is interactable, false is item
    {
        if (interClasses.TryGetValue(id, out _)) { return true; }
        else if (ItemManager.Instance.Items.TryGetValue(id, out _)) { return false; }
        else { throw new NullReferenceException("Interactable/Item id = " + id + " doesn't exist on list"); }
    }
}
