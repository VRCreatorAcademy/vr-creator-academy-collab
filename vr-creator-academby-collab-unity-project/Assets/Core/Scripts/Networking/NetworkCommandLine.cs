using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Management;

public class NetworkCommandLine : MonoBehaviour
{
    [SerializeField] private bool defaultSupressXR = false;

    private GameObject sceneCamera;
    private GameObject xrOrigin;
    private NetworkManager netManager;

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
        sceneCamera = GameObject.Find("Camera");
        if (sceneCamera == null) Debug.Log("Could not find: Camera");
        xrOrigin = GameObject.Find("XR Origin");
        if (xrOrigin == null) Debug.Log("Could not find: XR Origin");
    }
    public void StartXR()
    {
        Debug.Log("Starting XR...");
        findCameras();
        StartCoroutine(StartXRCoroutine());
        sceneCamera.SetActive(false);
        xrOrigin.SetActive(true);
        Debug.Log("XR started completely.");
    }
    public void StopXR()
    {
        if(XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            Debug.Log("Stopping XR...");
            findCameras();
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            sceneCamera.SetActive(true);
            xrOrigin.SetActive(false);
            Debug.Log("XR stopped completely.");
        }
        else
        {
            Debug.Log("Cannot stop XR because it was not initialized...");
        }
    }

    void Start()
    {
        netManager = GetComponentInParent<NetworkManager>();

        var args = GetCommandlineArgs();

        bool supressXR = defaultSupressXR;
        if (args.TryGetValue("-xr", out string xrValue))
        {
            switch (xrValue)
            {
                case "supress":
                    supressXR = true;
                    break;
            }
        }
        if (!supressXR)
        {
            // Turn on XR Plug-in
            StartXR();
        }
        else
        {
            findCameras();
            sceneCamera.SetActive(true);
            xrOrigin.SetActive(false);
        }


        if (Application.isEditor) return;

        if (args.TryGetValue("-mlapi", out string mlapiValue))
        {
            switch (mlapiValue)
            {
                case "server":
                    netManager.StartServer();
                    break;
                case "host":
                    netManager.StartHost();
                    break;
                case "client":
                    netManager.StartClient();
                    break;
            }
        }
    }

    private void OnApplicationQuit()
    {
        StopXR();
    }

    private Dictionary<string, string> GetCommandlineArgs()
    {
        Dictionary<string, string> argDictionary = new Dictionary<string, string>();

        var args = System.Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; ++i)
        {
            var arg = args[i].ToLower();
            if (arg.StartsWith("-"))
            {
                var value = i < args.Length - 1 ? args[i + 1].ToLower() : null;
                value = (value?.StartsWith("-") ?? false) ? null : value;

                argDictionary.Add(arg, value);
            }
        }
        return argDictionary;
    }
}