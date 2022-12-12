using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRMenu : MonoBehaviour
{
    public void StartXR()
    {
        XRManager xrManager = GameObject.Find("XR Manager").GetComponent<XRManager>();
        xrManager.StartXR();
    }
    public void StopXR()
    {
        XRManager xrManager = GameObject.Find("XR Manager").GetComponent<XRManager>();
        xrManager.StopXR();
    }

}
