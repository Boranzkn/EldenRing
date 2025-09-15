using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CharacterManager : NetworkBehaviour
{
    private const string DEATH_ANIMATION = "Death";

    [HideInInspector] public Animator Animator { private set; get; }

    [HideInInspector] public CharacterNetworkManager CharacterNetworkManager { private set; get; }
    [HideInInspector] public CharacterEffectsManager CharacterEffectsManager { private set; get; }
    [HideInInspector] public CharacterAnimatorManager CharacterAnimatorManager { private set; get; }

    protected CharacterController characterController;

    [Header("Status")]
    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Flags")]
    public bool isPerformingAction = false;
    public bool isJumping = false;
    public bool isGrounded = true;
    public bool applyRootMotion = false;
    public bool canRotate = true;
    public bool canMove = true;

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);

        Animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        CharacterNetworkManager = GetComponent<CharacterNetworkManager>();
        CharacterEffectsManager = GetComponent<CharacterEffectsManager>();
        CharacterAnimatorManager = GetComponent<CharacterAnimatorManager>();
    }

    protected virtual void Update()
    {
        Animator.SetBool("IsGrounded", isGrounded);

        if (IsOwner)
        {
            CharacterNetworkManager.networkPosition.Value = transform.position;
            CharacterNetworkManager.networkRotation.Value = transform.rotation;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(
                transform.position, 
                CharacterNetworkManager.networkPosition.Value, 
                ref CharacterNetworkManager.networkPositionVelocity, 
                CharacterNetworkManager.networkPositionSmoothTime);

            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                CharacterNetworkManager.networkRotation.Value, 
                CharacterNetworkManager.networkRotionSmoothTime);
        }
    }

    protected virtual void LateUpdate()
    {

    }

    public CharacterController GetCharacterController()
    {
        return characterController;
    }

    public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        if (IsOwner)
        {
            CharacterNetworkManager.currentHealth.Value = 0;
            isDead.Value = true;

            if (!manuallySelectDeathAnimation)
            {
                CharacterAnimatorManager.PlayTargetActionAnimation(DEATH_ANIMATION, true);
            }
        }

        yield return new WaitForSeconds(5);
    }

    public virtual void ReviveCharacter()
    {

    }
}
