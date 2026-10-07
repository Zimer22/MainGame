using System;
using System.Threading;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerMovement : MonoBehaviour
{
    Player player;
    Camera playerCam;
    GameObject interactHb;
    [SerializeField] float _speed;
    float _jumpHeight; // do we even need a jump?
    float sens;
    float rotationX;
    float rotationY;
    InputAction look;
    InputAction move;
    InputAction jump;
    void Start()
    {
        player = GetComponentInParent<Player>();
        sens = 1; //temp
        playerCam = player.GetCam();
        interactHb = player.GetPlayerInteract().gameObject;
        move = InputSystem.actions.FindAction("Move");
        jump = InputSystem.actions.FindAction("Jump");
        look = InputSystem.actions.FindAction("Look");
        JumpHeight = 20f;
    }

    void Update()
    {
        //Movement
        if(move.enabled)
        {
        Vector3 grounded = move.ReadValue<Vector2>() * Speed;
        float jumping = Convert.ToSingle(jump.IsPressed()) * JumpHeight; //todo grounddetector
        Vector3 movement = new Vector3(grounded.x,jumping,grounded.y);
        transform.parent.Translate(movement * Time.deltaTime, Space.Self);
        }
        //Movement

        //Sight
        if (look.enabled)
        {
            Vector2 mouse = look.ReadValue<Vector2>();
            rotationX += mouse.x * sens;
            rotationY += mouse.y * sens;
            rotationY = Math.Clamp(rotationY, -90f, 90f);
            playerCam.transform.localEulerAngles = new Vector3(-rotationY, 0, 0); // camera moves on the x
            transform.parent.localEulerAngles = new Vector3(0, rotationX, 0); //body moves on the y
        }
        //Sight

        //interactionHB
        Vector3 Interactposition = new Vector3(playerCam.transform.position.x, playerCam.transform.position.y, playerCam.transform.position.z);
        interactHb.transform.position = playerCam.transform.TransformPoint(Vector3.forward);
        interactHb.transform.rotation = playerCam.transform.rotation;
        //interactionHB
    }
    public void ToggleMovementLock()
    {
        if (look.enabled) 
        {
            look.Disable();
            move.Disable();
        }
        else
        {
            look.Enable();
            move.Enable();
        }
    }
    public float Speed 
    {
        get { return _speed; }
        set { _speed = value; }
    }
    public float JumpHeight
    {
        get { return _jumpHeight; }
        set { _jumpHeight = value; }
    }
}
