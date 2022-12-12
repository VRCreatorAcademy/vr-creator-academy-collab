using System.Collections;
using UnityEngine;
using UnityEngine.XR.Management;

public class XRManager : MonoBehaviour
{
    [SerializeField] private string sceneCameraName = "Camera";
    [SerializeField] private string xrOriginName = "XR Origin";

    private GameObject sceneCamera;
    private GameObject xrOrigin;

    private void OnApplicationQuit()
    {
        StopXR();
    }
    private IEnumerator StartXRCoroutine()
    {
        Debug.Log("Initializing XR...");
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.LogError("Initializing XR Failed. Check Editor or Player log for details.");
        }
        else
        {
            Debug.Log("Starting XR...");
            XRGeneralSettings.Instance.Manager.StartSubsystems();
        }
    }
    public void findCameras()
    {
        sceneCamera = GameObject.Find(sceneCameraName);
        if (sceneCamera == null) Debug.LogError("Could not find camera:" + sceneCameraName);
        xrOrigin = GameObject.Find(xrOriginName);
        if (xrOrigin == null) Debug.LogError("Could not find XR Origin:" + xrOriginName);
    }
    public void StartXR()
    {
        Debug.Log("Starting XR...");
        StartCoroutine(StartXRCoroutine());
        Debug.Log("XR started completely.");

        findCameras();
        sceneCamera.SetActive(false);
        xrOrigin.SetActive(true);
    }
    public void StopXR()
    {
        if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            Debug.Log("Stopping XR...");
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            Debug.Log("XR stopped completely.");
        }
        else
        {
            Debug.Log("Cannot stop XR because it was not initialized...");
        }

        findCameras();
        sceneCamera.SetActive(true);
        xrOrigin.SetActive(false);
    }
}
