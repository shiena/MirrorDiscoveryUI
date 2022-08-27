using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Mirror;
using Mirror.Discovery;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiscoveryUI : MonoBehaviour
{
    private readonly Dictionary<long, ServerResponse> _discoveredServers = new Dictionary<long, ServerResponse>();
    [SerializeField] private NetworkDiscovery networkDiscovery;
    [SerializeField] private TextMeshProUGUI discoveredServersCount;
    [SerializeField] private GameObject scrollViewContent;
    [SerializeField] private ConnectButton buttonPrefab;
    [SerializeField] private Button stopHostButton;
    [SerializeField] private Button stopClientButton;
    [SerializeField] private Button stopServerButton;

    /// <summary>
    /// Find Serversボタン
    /// </summary>
    public void FindServers()
    {
        _discoveredServers.Clear();
        networkDiscovery.StartDiscovery();
    }

    /// <summary>
    /// Start Hostボタン
    /// </summary>
    public void StartHost()
    {
        _discoveredServers.Clear();
        NetworkManager.singleton.StartHost();
        networkDiscovery.AdvertiseServer();
    }

    /// <summary>
    /// Start Serverボタン
    /// </summary>
    public void StartServer()
    {
        _discoveredServers.Clear();
        NetworkManager.singleton.StartServer();
        networkDiscovery.AdvertiseServer();
    }

    private void OnDiscoveredServer(ServerResponse info)
    {
        // Note that you can check the versioning to decide if you can connect to the server or not using this method
        _discoveredServers[info.serverId] = info;
        discoveredServersCount.text = $"Discovered Servers [{_discoveredServers.Count}]:";
        var button = Instantiate(buttonPrefab, scrollViewContent.transform);
        button.Initialize(info.EndPoint.Address.ToString(), () => Connect(info));
    }

    private void Connect(ServerResponse info)
    {
        networkDiscovery.StopDiscovery();
        NetworkManager.singleton.StartClient(info.uri);
    }

    private void StopButtons()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            stopHostButton.gameObject.SetActive(true);
            stopClientButton.gameObject.SetActive(false);
            stopServerButton.gameObject.SetActive(false);
        }
        else if (NetworkClient.isConnected)
        {
            stopHostButton.gameObject.SetActive(false);
            stopClientButton.gameObject.SetActive(true);
            stopServerButton.gameObject.SetActive(false);
        }
        else if (NetworkServer.active)
        {
            stopHostButton.gameObject.SetActive(false);
            stopClientButton.gameObject.SetActive(false);
            stopServerButton.gameObject.SetActive(true);
        }
        else
        {
            stopHostButton.gameObject.SetActive(false);
            stopClientButton.gameObject.SetActive(false);
            stopServerButton.gameObject.SetActive(false);
        }
    }

    private void StopHost()
    {
        NetworkManager.singleton.StopHost();
        networkDiscovery.StopDiscovery();
    }

    private void StopClient()
    {
        NetworkManager.singleton.StopClient();
        networkDiscovery.StopDiscovery();
    }

    private void StopServer()
    {
        NetworkManager.singleton.StopServer();
        networkDiscovery.StopDiscovery();
    }

    private void Start()
    {
        networkDiscovery.OnServerFound.AddListener(OnDiscoveredServer);
        stopHostButton.onClick.AddListener(StopHost);
        stopClientButton.onClick.AddListener(StopClient);
        stopServerButton.onClick.AddListener(StopServer);
    }

    private void OnDestroy()
    {
        networkDiscovery.OnServerFound.RemoveListener(OnDiscoveredServer);
        stopHostButton.onClick.RemoveListener(StopHost);
        stopClientButton.onClick.RemoveListener(StopClient);
        stopServerButton.onClick.RemoveListener(StopServer);
    }

    private void Update()
    {
        StopButtons();
        TestOnDiscoveredServer();
    }

    [Conditional("UNITY_EDITOR")]
    private void TestOnDiscoveredServer()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            var info = new ServerResponse()
            {
                EndPoint = new IPEndPoint(0, 80),
                serverId = _discoveredServers.Count + 1
            };
            OnDiscoveredServer(info);
        }
    }
}