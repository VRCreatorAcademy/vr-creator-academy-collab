using Unity.Netcode;
using UnityEngine.SceneManagement;

public class SceneLauncher : NetworkBehaviour
{
    public void ChangeScene(string SceneName)
    {
      // todo 2: Do not try to access the NetworkSceneManager when the NetworkManager
      // is shutdown.The NetworkSceneManager is only instantiated when a NetworkManager
      // is started.This same rule is true for all Netcode systems that reside within
      // the NetworkManager.
      NetworkManager.Singleton.SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
    }
}
