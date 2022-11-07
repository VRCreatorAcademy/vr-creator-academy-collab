using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayer : NetworkBehaviour
{
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public InputAction playerControls;

    private GameObject xr;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            xr = GameObject.Find("XR Origin");
            if (xr == null) Debug.Log("Was not able to find the XR Origin");

            Debug.Log("OnEnable IsOwner");
            playerControls.Enable();
        }
    }

    void Update()
    {
        if (IsOwner && IsClient)
        {
            if( xr == null )
            {
                Vector3 new_vector = playerControls.ReadValue<Vector3>() * 5.0f;
                Position.Value += new_vector * Time.deltaTime;
            }
            else
            {
                Position.Value = xr.transform.position;
            }
            //Debug.Log("Client " + Position.Value);
        }

        if (IsServer)
        {
            transform.position = Position.Value;
            //Debug.Log("Server " + Position.Value);
        }
    }
}
