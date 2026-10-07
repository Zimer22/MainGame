using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.InputSystem;
using UnityEngine.WSA;
using UnityEditor;

public class Player : NetworkBehaviour
{
    [SerializeField] float _health;
    Dictionary<int, Inventory> _inventories = new();
    public List<GameObject> attachedLists = new(); //temp
    NetworkVariable<float> netHealth = new();
    CameraManager cameraManager;
    [SerializeField] GameObject interactionHB;
    [SerializeField] GameObject playerCam;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerMenuUI _menuUI;
    [SerializeField] Camera cam;
    [SerializeField] PlayerInteract interact;
    InputAction menu;
    bool active;
    void Start()
    {
        netHealth.OnValueChanged += SynchHealth;
        if (IsOwner)
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.name == "PlayerText") { child.gameObject.SetActive(false); }
                else if (child.gameObject.name != "PlayerCam") { child.gameObject.SetActive(true); }
            }
            GetComponent<MeshRenderer>().enabled = false;
            CameraManager.Instance.ChangeCameras(cam);
            CanInteract = true; //debug
            menu = InputSystem.actions.FindAction("Menu");
            menu.performed += Menu_performed;
            menu.canceled += Menu_canceled;
        }
        foreach (var inventory in gameObject.GetComponents<Inventory>()) //temp
        {
            AddInventory(inventory);
        }
        _menuUI.Grids[0].PlayerStarted();
    }
    void Menu_performed(InputAction.CallbackContext obj)
    {
        if (active == true)
        {
            ClosePlayerMenuRpc();
        }
        else
        {
            OpenPlayerMenuRpc();
        }
    }
    void Menu_canceled(InputAction.CallbackContext obj)
    {

    }
    private new void OnDestroy()
    {
        netHealth.OnValueChanged -= SynchHealth;
        menu.performed -= Menu_performed;
        menu.canceled -= Menu_canceled;
        print("playerDestroyed");
    }
    public float Health
    {
        get
        {
            return _health;
        }
        set
        {
            if (IsServer)
            {
                _health = value;
                netHealth.Value = _health;
            }
        }
    }
    public PlayerMenuUI MenuUi
    {
        get 
        {
            return _menuUI;  
        }
        set 
        { 
            _menuUI = value; 
        }
    }
    public bool CanInteract 
    { 
        get
        {
            return interact.CanInteract;
        }
        set
        {
            interact.CanInteract = value;
        }
    }
    public Dictionary<int, Inventory> Inventories
    {
        get
        {
            return _inventories;
        }
        set
        {
            _inventories = value;
        }
    }
    public void AddInventory(Inventory inv) // why is this a dictionary
    {
        int id = _inventories.Count;
        _inventories.Add(id, inv);
    }
    [Rpc(SendTo.Everyone)]
    public void OpenPlayerMenuRpc()
    {
        MenuUi.transform.position = transform.position;
        MenuUi.transform.Translate(0f, 0.6f, 0.9f, Space.Self);
        MenuUi.gameObject.SetActive(true);
        if(IsOwner)
        {
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
            playerMovement.ToggleMovementLock();
            cam.fieldOfView = 50f;
        }
        playerCam.transform.Rotate(-playerCam.transform.eulerAngles.x, 0f, 0f);
        active = true;
    }
    [Rpc(SendTo.Everyone)]
    public void ClosePlayerMenuRpc()
    {
        attachedLists.ForEach(grid => grid.SetActive(false));
        attachedLists.Clear();
        MenuUi.gameObject.SetActive(false);
        if (IsOwner)
        {
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            playerMovement.ToggleMovementLock();
            cam.fieldOfView = 50f;
        }
        playerCam.transform.Rotate(-playerCam.transform.eulerAngles.x, 0f, 0f);
        active = false;
    }
    public void SynchHealth(float prevvalue,float newvalue)
    {
        if (!IsServer)
        {
            _health = netHealth.Value;
        }
    }
    public PlayerInteract GetPlayerInteract()
    {
        return interact;
    }
    public Camera GetCam()
    {
        return cam;
    }
    
}
 