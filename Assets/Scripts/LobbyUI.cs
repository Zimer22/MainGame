using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

    /// <summary>
    /// Add this component to the same GameObject as
    /// the NetworkManager component.
    /// </summary>
    public class LobbyUI : MonoBehaviour
    {
        private NetworkManager mNetworkManager;
        UnityTransport transport;
    [SerializeField] TextMeshProUGUI ip;
    [SerializeField] TextMeshProUGUI sens;
        ushort port = 7777;
        private void Start()
        {
        mNetworkManager = NetworkManager.Singleton;
        transport = mNetworkManager.GetComponent<UnityTransport>();
    }
    public void Host()
    {
        mNetworkManager.StartHost();
        this.gameObject.SetActive(false);
    }
    public void Client()
    {
        mNetworkManager.transform.GetComponent<UnityTransport>().SetConnectionData("127.0.0.1", port);
        mNetworkManager.StartClient();
        this.gameObject.SetActive(false);
    }
    public void Server()
    {
        mNetworkManager.StartServer();
        this.gameObject.SetActive(false);
    }
        public float GetSens()
        {
        return float.Parse(sens.text);
        }
    }