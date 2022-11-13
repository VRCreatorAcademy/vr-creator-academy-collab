using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Management;

public class NetworkCommandLine : MonoBehaviour
{
    [SerializeField] private bool defaultSupressXR = false;

    private NetworkManager netManager;

    public IEnumerator StartXR()
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
    public void StopXR()
    {
        Debug.Log("Stopping XR...");

        XRGeneralSettings.Instance.Manager.StopSubsystems();
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        Debug.Log("XR stopped completely.");
    }

    void Start()
    {
        netManager = GetComponentInParent<NetworkManager>();

        GameObject camera = GameObject.Find("Camera");
        if (camera == null) Debug.Log("Could not find: Camera");
        GameObject xrOrigin = GameObject.Find("XR Origin");
        if (xrOrigin == null) Debug.Log("Could not find: XR Origin");

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
            Debug.Log("Starting XR");
            StartCoroutine(StartXR());
            camera.SetActive(false);
            xrOrigin.SetActive(true);
        }
        else
        {
            camera.SetActive(true);
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