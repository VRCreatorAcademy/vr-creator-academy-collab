using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayer : NetworkBehaviour
{
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner );
    public InputAction playerControls;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Debug.Log("OnEnable IsOwner");
            playerControls.Enable();
        }
    }

    void Update()
    {
        if( IsOwner && IsClient)
        {
            Vector3 new_vector = playerControls.ReadValue<Vector3>() / 10.0f;
            Position.Value += new_vector;
            Debug.Log("Client " + Position.Value);
        }

        if (IsServer)
        {
            transform.position = Position.Value;
            Debug.Log("Server " + Position.Value);

        }
    }
}
