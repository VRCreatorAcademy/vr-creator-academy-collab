using System.Collections;
using UnityEngine;
using UnityEngine.XR.Management;

/// <summary>
// What is going on in this class is documented in the following Unity documentation:
// https://docs.unity3d.com/Packages/com.unity.xr.management@4.2/manual/EndUser.html
/// </summary>

public class XRManager : MonoBehaviour
{

    [SerializeField] private string sceneCameraName = "Camera";
    [SerializeField] private string xrOriginName = "XR Origin";

    [SerializeField] private GameObject sceneCamera;
    [SerializeField] private GameObject xrOrigin;

    public enum ControlMode
    {
        XReality,
        MouseKeyboard
    }
    public ControlMode controlMode = ControlMode.MouseKeyboard;

    private void Update()
    {
        // todo: 3 We should only need to do this once when we changes scenes.
        if (sceneCamera == null) sceneCamera = GameObject.Find(sceneCameraName);
        if (xrOrigin == null) xrOrigin = GameObject.Find(xrOriginName);

        switch (controlMode)
        {
            case ControlMode.MouseKeyboard:
                if (findCameras())
                {
                    sceneCamera?.SetActive(true);
                    xrOrigin?.SetActive(false);
                }
                break;
            case ControlMode.XReality:
                if (findCameras())
                {
                    sceneCamera?.SetActive(false);
                    xrOrigin?.SetActive(true);
                }
                break;
        }
    }
    private void OnApplicationQuit()
    {
        StopXR();
    }
    private IEnumerator StartXRCoroutine()
    {
        Debug.Log("Initializing XR...");

        // todo: 5 Probably should remove the following code since its dead and prevents game from building.
        // Found the following code some where, it seems reasonable but it doesn't build -J. Christ
        //if (XRGeneralSettings.Instance == null)
        //{
        //    XRGeneralSettings.Instance = ScriptableObject.CreateInstance<XRGeneralSettings>();
        //    XRGeneralSettings.Instance.Manager = ScriptableObject.CreateInstance<XRManagerSettings>();
        //    Debug.LogWarning("XRManagerSettings created...");
        //}

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
    public bool findCameras()
    {
        //sceneCamera = GameObject.Find(sceneCameraName);
        if (sceneCamera == null) Debug.LogWarning("Could not find camera");
        //xrOrigin = GameObject.Find(xrOriginName);
        if (xrOrigin == null) Debug.LogWarning("Could not find XR Origin");
        if (sceneCamera == null || xrOrigin == null) return false;
        return true;
    }
    public void StartXR()
    {
        controlMode = ControlMode.XReality;

        Debug.Log("Starting XR Coroutine...");

        if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            Debug.Log("XR already started.");
        }
        else
        {
            StartCoroutine(StartXRCoroutine());
        }

        if (findCameras())
        {
            sceneCamera?.SetActive(false);
            xrOrigin?.SetActive(true);
        }
    }
    public void StopXR()
    {
        controlMode = ControlMode.MouseKeyboard;

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

        if (findCameras())
        {
            sceneCamera?.SetActive(true);
            xrOrigin?.SetActive(false);
        }

    }
}
