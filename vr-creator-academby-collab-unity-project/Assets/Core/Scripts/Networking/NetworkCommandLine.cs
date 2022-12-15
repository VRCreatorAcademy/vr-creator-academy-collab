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
    private XRManager xrManager;

    void Start()
    {
        Debug.Log("NetworkCommandLine Start()");

        netManager = GetComponentInParent<NetworkManager>();
        if (netManager == null) Debug.Log("Could not find NetworkManager");
        xrManager = GameObject.Find("XR Manager").GetComponent<XRManager>();
        if (xrManager == null) Debug.Log("Could not find XRManager");

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
            xrManager.StartXR(); // Turn on XR Plug-in
        }
        else
        {
            xrManager.StopXR();
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
        Debug.Log("OnApplicationQuit()");
        xrManager.StopXR();
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