using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerText : MonoBehaviour
{
    Canvas canvas;
    TextMeshProUGUI text;
    Player otherPlayer; 

    void Start()
    {
        text = transform.GetComponent<TextMeshProUGUI>();
        canvas = transform.GetComponent<Canvas>();
        if (transform.parent.GetComponent<NetworkObject>().IsOwner)
        {
            this.enabled = false;
        } 
        else otherPlayer = transform.parent.GetComponent<Player>();
    }
    void Update()
    {
         text.SetText("Health: " + otherPlayer.Health);
         Vector3 backwards = new Vector3(0, -180, 0);
         transform.LookAt(CameraManager.Instance.ActiveCamera.transform.position);
         transform.Rotate(backwards);
    }
} 
