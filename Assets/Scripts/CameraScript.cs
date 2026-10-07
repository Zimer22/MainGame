using UnityEngine;
[RequireComponent(typeof(Camera))]
public class CameraScript : MonoBehaviour
{
    public Tag Tag;
    CameraManager manager;
    new Camera camera;
    Player player;
    void Start()
    {
        manager = CameraManager.Instance;
        camera = GetComponent<Camera>();
        manager.AddCamera(this, camera);
    }
    public Player Player
    {
        get 
        {
            if (player == null)
            { player = transform.parent.GetComponent<Player>(); }
            return player; 
        }
        set { player = value; }
    }
}
