using Unity.Netcode;
using UnityEngine;

public class CharacterNetworkManager : NetworkBehaviour
{
    CharacterManager character;

    [Header("Position")]
    public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public Vector3 networkPositionVelocity;
    public float networkPositionSmoothTime = 0.1f;
    public float networkRotionSmoothTime = 0.1f;

    [Header("Animation")]
    public NetworkVariable<float> horizontalMovement = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> verticalMovement = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> moveAmount = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Flags")]
    public NetworkVariable<bool> isSprinting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isJumping = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Resources")]
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> maxHealth = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> currentStamina = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> maxStamina = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Stats")]
    public NetworkVariable<int> vitality = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> endurance = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public void CheckHP(int oldValue, int newValue)
    {
        if (currentHealth.Value <= 0)
        {
            StartCoroutine(character.ProcessDeathEvent());
        }

        if (character.IsOwner && currentHealth.Value > maxHealth.Value)
        {
            currentHealth.Value = maxHealth.Value;
        }
    }

    [ServerRpc]
    public void PlayActionAnimationServerRpc(ulong clientID, string animationID, bool applyRootMotion)
    {
        if (IsServer)
        {
            PlayActionAnimationClientRpc(clientID, animationID, applyRootMotion);
        }
    }

    [ClientRpc]
    private void PlayActionAnimationClientRpc(ulong clientID, string animationID, bool applyRootMotion)
    {
        if (clientID != NetworkManager.Singleton.LocalClientId)
        {
            character.applyRootMotion = applyRootMotion;
            character.Animator.CrossFade(animationID, 0.2f);
        }
    }

    [ServerRpc]
    public void PlayAttackActionAnimationServerRpc(ulong clientID, string animationID, bool applyRootMotion)
    {
        if (IsServer)
        {
            PlayAttackActionAnimationClientRpc(clientID, animationID, applyRootMotion);
        }
    }

    [ClientRpc]
    private void PlayAttackActionAnimationClientRpc(ulong clientID, string animationID, bool applyRootMotion)
    {
        if (clientID != NetworkManager.Singleton.LocalClientId)
        {
            character.applyRootMotion = applyRootMotion;
            character.Animator.CrossFade(animationID, 0.2f);
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void NotifyTheServerOfCharacterDamageServerRpc(
        ulong damagedCharacterID,
        ulong characterCausingDamageID,
        float physicalDamage,
        float magicDamage,
        float fireDamage,
        float holyDamage,
        float lightninDamage,
        float angleHitFrom,
        float contactPointX,
        float contactPointY,
        float contactPointZ)
    {
        NotifyTheServerOfCharacterDamageClientRpc(damagedCharacterID, characterCausingDamageID, physicalDamage, magicDamage, fireDamage, holyDamage, lightninDamage, angleHitFrom, contactPointX, contactPointY, contactPointZ);
    }

    [ClientRpc]
    private void NotifyTheServerOfCharacterDamageClientRpc(
        ulong damagedCharacterID,
        ulong characterCausingDamageID,
        float physicalDamage,
        float magicDamage,
        float fireDamage,
        float holyDamage,
        float lightninDamage,
        float angleHitFrom,
        float contactPointX,
        float contactPointY,
        float contactPointZ)
    {
        ProcessCharacterDamageFromServer(damagedCharacterID, characterCausingDamageID, physicalDamage, magicDamage, fireDamage, holyDamage, lightninDamage, angleHitFrom, contactPointX, contactPointY, contactPointZ);
    }

    public void ProcessCharacterDamageFromServer(
       ulong damagedCharacterID,
       ulong characterCausingDamageID,
       float physicalDamage,
       float magicDamage,
       float fireDamage,
       float holyDamage,
       float lightninDamage,
       float angleHitFrom,
       float contactPointX,
       float contactPointY,
       float contactPointZ)
    {
        CharacterManager damagedCharacter = NetworkManager.Singleton.SpawnManager.SpawnedObjects[damagedCharacterID].gameObject.GetComponent<CharacterManager>();
        CharacterManager characterCausingDamage = NetworkManager.Singleton.SpawnManager.SpawnedObjects[characterCausingDamageID].gameObject.GetComponent<CharacterManager>();
        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.Instance.takeDamageEffect);

        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.holyDamage = holyDamage;
        damageEffect.lightningDamage = lightninDamage;
        damageEffect.angleHitFrom = angleHitFrom;
        damageEffect.contactPoint = new Vector3(contactPointX, contactPointY, contactPointZ);
        damageEffect.characterCausingDamage = characterCausingDamage;

        damagedCharacter.CharacterEffectsManager.ProcessInstantEffect(damageEffect);
    }
}
