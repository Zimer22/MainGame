using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public abstract class Interactable : NetworkBehaviour
{
    protected bool isInventory;
    protected float cooldown;
    protected float baseCooldown;
    protected bool isInteracting; //this is handled by individual objects
    protected bool canCancel;
    protected delegate void InteractableDelegate(ulong player,int id);
    protected event InteractableDelegate beginInteract;
    protected event InteractableDelegate cancelInteract;
    protected event InteractableDelegate finishInteract;
    protected Dictionary<int, string> interactions = new Dictionary<int, string>();
    protected ObjectManager manager;
    protected virtual void InteractedCall(ulong player,int id)
    {
        beginInteract?.Invoke(player,id);
    }
    protected virtual void CancelInteractionCall(ulong player, int id)
    {
       cancelInteract?.Invoke(player, id);
    }
    protected virtual void FinishedInteractionCall(ulong player, int id)
    {
        finishInteract?.Invoke(player, id);
    }
    public void BeginInteraction(ulong player,int id) 
    {
        if (cooldown <= 0f && !isInteracting)
        {
            print("interacting");
            cooldown = baseCooldown;
            InteractedCall(player,id);
        }
    }
    public void CancelInteraction(ulong player,int id)
    {
        if (isInteracting && canCancel)
        {
            CancelInteractionCall(player,id);
        }
    
    }
    public void FinishedInteraction(ulong player, int id)
    { 
        FinishedInteractionCall(player, id); //this would be used by other objects to trigger something in response (as the item already knows when it stopped interacting)
    }

    public Dictionary<int, string> GetInteractions()
    {
        return interactions;
    }
    public bool GetIsInteracting()
    {
        return isInteracting;
    }
    protected void AddToManager()
    {
        ObjectManager.Instance.AddInteractable(this);
    }
    private void Update()
    {
        cooldown -= Time.deltaTime;
    }
}