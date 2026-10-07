using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;

public class DamagingCube : Interactable
{
    Renderer localrenderer;
    float damage = 5f;
    private void Start()
    {
        localrenderer = transform.GetComponent<Renderer>();
        interactions.Add(0,"Toggle Color");
        interactions.Add(1, "Take Damage");
        interactions.Add(2, "Damage random player");
        interactions.Add(3, "Drop Random Item");
        beginInteract += ClientInteractRpc;
    }
    [Rpc(SendTo.Server)]
    private void ClientInteractRpc(ulong playerid, int id)
    {
        Player player = GetNetworkObject(playerid).GetComponent<Player>();
        if (player.CanInteract)
        {
            DamagingCubeInteractRpc(playerid, id);
        }
        else { print("player dead cant interact"); }
    }

    [Rpc(SendTo.Everyone)]
    private void DamagingCubeInteractRpc(ulong playerid, int id) //this is debug, i should not handle toggle states like colour with rpc, late joining clients would be desynced
    {
        Player player = GetNetworkObject(playerid).GetComponent<Player>();
        switch (id) 
        { 
            case 0:
                if(localrenderer.material.color == Color.red)
                {
                    localrenderer.material.color = Color.gray;
                }
                else { localrenderer.material.color = Color.red; }
                break;
            case 1:
                player.Health += damage;
                print("tried damage host");
                break;
            case 2:
                List<NetworkClient>connectedPlayers = (List<NetworkClient>)GetNetworkObject(playerid).NetworkManager.ConnectedClientsList;
                player = connectedPlayers[Random.Range(0, connectedPlayers.Count)].PlayerObject.GetComponent<Player>();
                player.Health += damage;
                print("tried damage random");
                break;
            case 3:
                player.Inventories[0].DropInvItem(0);
                break;
        }
    }
}
