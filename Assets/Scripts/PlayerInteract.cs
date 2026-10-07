using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInteract : MonoBehaviour
{
    Collider interactionHb;
    Player player;
    Camera localPlayerCam;
    DynamicUI interactableHud;
    RaycastHit hit;
    int prevhit;
    Interactable seenScript;
    Dictionary<int, string> interactions = new Dictionary<int, string>();
    List<KeyValuePair<int, string>> interactionsOrder = new List<KeyValuePair<int, string>>();
    InputAction interact;
    InputAction scroll;
    ObjectManager manager;
    int selectedInteraction;
    int objectsInCollider;
    bool eKey;
    bool eKeyHold;
    bool cleared = true;
    bool _canInteract;
    float localScroll;
    private void Start()
    {
        manager = ObjectManager.Instance;
        interactionHb = GetComponent<Collider>();
        player = transform.parent.GetComponent<Player>();
        interactableHud = UIManager.Instance.DynamicUI; //debug
        localPlayerCam = player.GetCam();
        scroll = InputSystem.actions.FindAction("ScrollWheel");
        interact = InputSystem.actions.FindAction("Interact");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (manager.interactableObjNetIds.TryGetValue(other.gameObject.GetInstanceID(), out _))
        {
            objectsInCollider++;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (manager.interactableObjNetIds.TryGetValue(other.gameObject.GetInstanceID(), out _))
        {
            objectsInCollider--;
        }
    }
    private void UpdateHitData()
    {
        int idObj = hit.transform.gameObject.GetInstanceID();
        ulong netIdScr = manager.interactableObjNetIds[idObj];
        if (manager.IsInteractableOrItem(netIdScr)) 
        {
            manager.interClasses.TryGetValue(netIdScr, out seenScript);
        }
        else
        {
            Item itemRef;
            ItemManager.Instance.Items.TryGetValue(netIdScr, out itemRef);
            seenScript = itemRef;
        }
        interactions.AddRange(seenScript.GetInteractions()); //this needs to be called to refresh interactions. could be called once an _inventory function is done
        interactionsOrder.AddRange(interactions);
    }
    private void UpdateInteractionData()
    {
    }
    private void ClearHitData()
    {
        interactableHud.EmptySelectedObjectText();
        interactableHud.EmptySelectedInteractText();
        interactions.Clear();
        interactionsOrder.Clear();
        selectedInteraction = 0;
        cleared = true;
    }
    private bool IsInteractHeld() //could be better
    {
        try
        {
            return eKeyHold;
        }
        finally
        {
            if (eKey && !eKeyHold)
            {
                eKeyHold = true;
            }
            else if (!eKey) { eKeyHold = false; }
        }
    }
    public bool CanInteract
    {
        get { return _canInteract; }
        set { _canInteract = value; }
    }
    private void Update()
    {
        if (CanInteract && objectsInCollider > 0)
        {
            //Interact RayCast
            if (Physics.Raycast(localPlayerCam.transform.position, localPlayerCam.transform.forward, out hit, 1.4f, LayerMask.GetMask("Default")))
            {
                if (hit.colliderInstanceID != prevhit)
                {
                    ClearHitData();
                    if (!manager.interactableObjNetIds.TryGetValue(hit.transform.gameObject.GetInstanceID(), out _)) //check raycast is hitting correct object
                    {
                        Debug.DrawRay(localPlayerCam.transform.position, localPlayerCam.transform.forward * hit.distance, Color.red, 1f, false);
                    }
                    else
                    {
                        UpdateHitData();
                        Debug.DrawRay(localPlayerCam.transform.position, localPlayerCam.transform.forward * hit.distance, Color.blue, 3f, false);
                        cleared = false;
                    }
                }
                prevhit = hit.colliderInstanceID;
            }
            else
            {
                ClearHitData();
                prevhit = 0;
            }
            //Interact RayCast

            //Input
            localScroll = scroll.ReadValue<Vector2>().y;
            eKey = interact.WasPressedThisFrame();
            eKeyHold = IsInteractHeld();
            //Input

            //Interaction Selection
            if (interactions.Count > 0) //could be optimized with input system hooks
            {
                string selinteract = interactionsOrder[selectedInteraction].Value;
                string previnteract = interactionsOrder.ElementAtOrDefault(selectedInteraction - 1).Value;
                string nextinteract = interactionsOrder.ElementAtOrDefault(selectedInteraction + 1).Value;
                if (cleared == false)
                {
                    interactableHud.ChangeSelectedInteractText(selinteract, previnteract, nextinteract);
                    interactableHud.ChangeSelectedObjectText(hit.transform.gameObject.name);
                }
                if (selectedInteraction == 0 && -localScroll == -1)
                {
                }
                else if (selectedInteraction == interactionsOrder.Count - 1 && -localScroll == 1)
                {
                }
                else
                {
                    selectedInteraction += -(int)localScroll;
                }
            }
            //Interaction Selection

            //Interaction Start & Cancel
            if (interactions.Count > 0)
            {
                if (eKey && !eKeyHold) //could be optimized with input system hooks
                {
                    seenScript.BeginInteraction(player.NetworkObjectId, selectedInteraction);
                }
                else if (!eKey && eKeyHold)
                {
                    seenScript.CancelInteraction(player.NetworkObjectId, selectedInteraction);
                }
            }
            //Interaction Start & Cancel  
        }
        else if (cleared == false)
        {
            ClearHitData();
        }
    }
}
