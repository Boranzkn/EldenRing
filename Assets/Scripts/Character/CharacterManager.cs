using Unity.Netcode;
using UnityEngine;

public class CharacterManager : NetworkBehaviour
{
    protected CharacterController characterController;

    private CharacterNetworkManager characterNetworkManager;

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);

        characterController = GetComponent<CharacterController>();
        characterNetworkManager = GetComponent<CharacterNetworkManager>();
    }

    protected virtual void Update()
    {
        if (IsOwner)
        {
            characterNetworkManager.networkPosition.Value = transform.position;
            characterNetworkManager.networkRotation.Value = transform.rotation;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(
                transform.position, 
                characterNetworkManager.networkPosition.Value, 
                ref characterNetworkManager.networkPositionVelocity, 
                characterNetworkManager.networkPositionSmoothTime);

            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                characterNetworkManager.networkRotation.Value, 
                characterNetworkManager.networkRotionSmoothTime);
        }
    }

    protected virtual void LateUpdate()
    {

    }

    public CharacterController GetCharacterController()
    {
        return characterController;
    }
}
