using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
public class CameraManager : Singleton<CameraManager>
{
    Dictionary<ulong,Camera> playerCameras = new Dictionary<ulong, Camera>();
    Dictionary<int, Camera> mapCameras = new Dictionary<int, Camera>(); //add more dictionaries as needed
    NetworkManager networkManager;
    Camera prevCamera;
    public delegate void CameraDelegate(Camera camera);
    public event CameraDelegate OnCameraChange;
    void Start()
    {
        networkManager = NetworkManager.Singleton;
    }
    public void ChangeCameras(Camera cam)
    {
        if(prevCamera == null)
        {
            prevCamera = mapCameras[-1]; //assuming that the lobby camera is enabled and active
        }
        prevCamera.gameObject.SetActive(false); //camera SHOULD be in a separate object from the main otherwise this will kill all of it
        prevCamera = cam;
        cam.gameObject.SetActive(true);
        OnCameraChange?.Invoke(cam);
    }
    public void AddCamera(CameraScript camscript,Camera cam)
    {
        ulong playerId;
        int id;
        if (camscript.Tag == Tag.PlayerCamera)
        {
            playerId = camscript.Player.OwnerClientId;
            playerCameras.Add(playerId, cam);
        }
        else
        {
        if(camscript.Tag == Tag.LobbyCamera)
        {
            id = -1; //debug? should maybe separate it into another list
        }
        else { id = mapCameras.Count; }
        mapCameras.Add(id, cam);
        }
    }
    public Camera ActiveCamera
    {
        get 
        { 
            return prevCamera; 
        }
    }
}
