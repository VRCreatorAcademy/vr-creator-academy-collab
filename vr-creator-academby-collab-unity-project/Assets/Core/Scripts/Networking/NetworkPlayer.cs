using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayer : NetworkBehaviour
{
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public InputAction playerControls;

    private GameObject xr = null;
    private XRManager xrManager;

    public void Start()
    {
        xrManager = GameObject.Find("XR Manager").GetComponent<XRManager>();
        if (xrManager == null) Debug.LogError("Could not find referance to XRManager");
        else Debug.Log("Found referance to XRManager");
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner && IsClient)
        {
            xr = GameObject.Find("XR Origin");
            if (xr == null) Debug.Log("Was not able to find the XR Origin");
            else          
                Debug.Log("OnNetworkSpawn() IsOwner && IsClient: " + xr.activeSelf.ToString());

            playerControls.Enable();
        }
    }

    void Update()
    {
        if (IsOwner && IsClient)
        {
            if (xrManager.controlMode == XRManager.ControlMode.MouseKeyboard)
            {
                Vector3 new_vector = playerControls.ReadValue<Vector3>() * 5.0f;
                Position.Value += new_vector * Time.deltaTime;
                //Debug.Log("Client(XR) " + Position.Value);
            }
            else if(xrManager.controlMode == XRManager.ControlMode.XReality)
            {
                if (xr == null)
                {
                    xr = GameObject.Find("XR Origin");
                }
                if (xr == null)
                {
                    Debug.Log("Could not finx XR Origin in XReality mode");
                }
                else
                {
                    Position.Value = xr.transform.position;
                    //Debug.Log("Client(MK) " + Position.Value);
                }
            }
            else
            {
                Debug.LogError("Unknown control mode");
            }
        }

        if (IsServer)
        {
            transform.position = Position.Value;
            //Debug.Log("Server " + Position.Value);
        }
    }
}
