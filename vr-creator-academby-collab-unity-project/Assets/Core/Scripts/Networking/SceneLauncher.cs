using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLauncher : NetworkBehaviour
{
    [SerializeField] bool LaunchSceneOnStart = false;
    [SerializeField] string SceneName = "_StartMenu";

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.SceneManager.OnSceneEvent += SceneEventCallback_OnSceneEvent;
        }
        base.OnNetworkSpawn();

        if (LaunchSceneOnStart)
            ChangeScene(SceneName);
    }
    private void SceneEventCallback_OnSceneEvent(SceneEvent sceneEvent)
    {
        Debug.Log(sceneEvent.ToString());
    }
    public void ChangeScene(string SceneName)
    {
        // todo 2: Do not try to access the NetworkSceneManager when the NetworkManager
        // is shutdown.The NetworkSceneManager is only instantiated when a NetworkManager
        // is started.This same rule is true for all Netcode systems that reside within
        // the NetworkManager.

        //if (IsServer && !string.IsNullOrEmpty(SceneName))
        {
            var status = NetworkManager.Singleton.SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogWarning($"Failed to load {SceneName} " +
                        $"with a {nameof(SceneEventProgressStatus)}: {status}");
            }
        }
    }
}
