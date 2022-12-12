using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    [SerializeField] GameObject networkManagerGameObject;
    private string ip_address = "undefined";
    private UnityTransport utp;

    private void Start()
    {
        if (networkManagerGameObject != null)
        {
            utp = networkManagerGameObject.GetComponent<UnityTransport>();
            ip_address = utp.ConnectionData.Address;
        }
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
        }

        GUILayout.EndArea();
    }

    void StartButtons()
    {
        GUILayout.BeginHorizontal(); {
            GUILayout.Label("Server IP");
            ip_address = GUILayout.TextField(ip_address);
        } GUILayout.EndHorizontal();

        if (utp) utp.ConnectionData.Address = ip_address;

        if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
        if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();

        //if (GUILayout.Button("Test"))
        //{
        //    if (utp) Debug.Log("Address: " + utp.ConnectionData.Address);
        //    else Debug.Log("Network Manager not defined in the inspector");
        //}
    }

    static void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        if (GUILayout.Button("Menu"))
        {
            Debug.Log("Menu");
            string SceneName = "_StartMenu";
            var status = NetworkManager.Singleton.SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogWarning($"Failed to load {SceneName} " +
                        $"with a {nameof(SceneEventProgressStatus)}: {status}");
            }
        }

        GUILayout.Label("Transport: " +
            NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);

    }
}
