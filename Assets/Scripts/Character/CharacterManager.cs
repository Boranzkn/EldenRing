using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterManager : NetworkBehaviour
{
    private const string DEATH_ANIMATION = "Death";

    [HideInInspector] public Animator Animator { private set; get; }

    [HideInInspector] public CharacterNetworkManager CharacterNetworkManager { private set; get; }
    [HideInInspector] public CharacterEffectsManager CharacterEffectsManager { private set; get; }
    [HideInInspector] public CharacterAnimatorManager CharacterAnimatorManager { private set; get; }
    [HideInInspector] public CharacterCombatManager CharacterCombatManager { private set; get; }

    protected CharacterController characterController;

    [Header("Status")]
    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Flags")]
    public bool isPerformingAction = false;
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
        CharacterCombatManager = GetComponent<CharacterCombatManager>();
    }

    protected virtual void Start()
    {
        IgnoreMyOwnColliders();
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

    protected virtual void IgnoreMyOwnColliders()
    {
        Collider characterControllerCollider = GetComponent<Collider>();
        Collider[] damageableCharacterColliders = GetComponentsInChildren<Collider>();
        List<Collider> ignoreColliders = new List<Collider>();

        //  ADDS ALL OF OUR DAMAGEABLE CHARACTER COLLIDERS, TO THE LIST THAT WILL BE USED TO IGNORE COLLISIONS
        foreach (var collider in damageableCharacterColliders)
        {
            ignoreColliders.Add(collider);
        }

        //  ADDS THE CHARACTER CONTROLLER COLLIDER, TO THE LIST THAT WILL BE USED TO IGNORE COLLISIONS
        ignoreColliders.Add(characterControllerCollider);

        //  IGNORES COLLISIONS BETWEEN ALL OF THE COLLIDERS IN THE LIST
        foreach (var colliderA in ignoreColliders)
        {
            foreach (var colliderB in ignoreColliders)
            {
                Physics.IgnoreCollision(colliderA, colliderB, true);
            }
        }
    }
}
