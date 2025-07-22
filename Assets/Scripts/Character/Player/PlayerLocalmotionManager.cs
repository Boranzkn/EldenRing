using UnityEngine;

public class PlayerLocalmotionManager : CharacterLocalmotionManager
{
    [HideInInspector] public float verticalMovement;
    [HideInInspector] public float horizontalMovement;
    [HideInInspector] public float moveAmount;

    [SerializeField] private float walkingSpeed = 2;
    [SerializeField] private float runningSpeed = 5;
    [SerializeField] private float sprintingSpeed = 7;
    [SerializeField] private float rotationSpeed = 15;
    [SerializeField] private int sprintingStaminaCost = 2;

    private PlayerManager player;
    private CharacterController characterController;
    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;
    private Vector3 rollDirection;
    private float dodgeStaminaCost = 25;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
        characterController = player.GetCharacterController();
    }

    protected override void Update()
    {
        base.Update();

        if (player.IsOwner)
        {
            player.characterNetworkManager.horizontalMovement.Value = horizontalMovement;
            player.characterNetworkManager.verticalMovement.Value = verticalMovement;
            player.characterNetworkManager.moveAmount.Value = moveAmount;
        }
        else
        {
            horizontalMovement = player.characterNetworkManager.horizontalMovement.Value;
            verticalMovement = player.characterNetworkManager.verticalMovement.Value;
            moveAmount = player.characterNetworkManager.moveAmount.Value;

            //  IF NOT LOCKED ON, PASS MOVE AMOUNT
            player.PlayerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.PlayerNetworkManager.isSprinting.Value);

            //  IF LOCKED ON, PASS HORIZONTAL AND VERTICAL
        }
    }

    public void HandleAllMovement()
    {
        HandleGroundedMovement();
        HandleRotation();
    }

    private void GetMovementValues()
    {
        verticalMovement = PlayerInputManager.Instance.GetVerticalInput();
        horizontalMovement = PlayerInputManager.Instance.GetHorizontalInput();
        moveAmount = PlayerInputManager.Instance.GetMoveAmount();
    }

    private void HandleGroundedMovement()
    {
        if (!player.canMove) return;

        GetMovementValues();

        moveDirection = PlayerCamera.Instance.transform.forward * verticalMovement;
        moveDirection += PlayerCamera.Instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (player.PlayerNetworkManager.isSprinting.Value)
        {
            characterController.Move(moveDirection * sprintingSpeed * Time.deltaTime);
        }
        else
        {
            if (PlayerInputManager.Instance.GetMoveAmount() > 0.5f)
            {
                // MOVE AT A RUNNING SPEED
                characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
            }
            else if (PlayerInputManager.Instance.GetMoveAmount() <= 0.5f)
            {
                // MOVE AT A WALKING SPEED
                characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
            }
        }
    }

    private void HandleRotation()
    {
        if (!player.canRotate) return;

        targetRotationDirection = Vector3.zero;
        targetRotationDirection = PlayerCamera.Instance.GetCamera().transform.forward * verticalMovement;
        targetRotationDirection += PlayerCamera.Instance.GetCamera().transform.right * horizontalMovement;
        targetRotationDirection.Normalize();
        targetRotationDirection.y = 0;

        if (targetRotationDirection == Vector3.zero)
        {
            targetRotationDirection = transform.forward;
        }

        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;
    }

    public void AttemptToPerformDodge()
    {
        if (player.isPerformingAction || player.PlayerNetworkManager.currentStamina.Value <= 0) return;

        //  IF WE ARE MOVING WHEN WE ATTEMPT TO DODGE, PERFORM ROLL
        if (PlayerInputManager.Instance.GetMoveAmount() > 0)
        {
            rollDirection = PlayerCamera.Instance.GetCamera().transform.forward * PlayerInputManager.Instance.GetVerticalInput();
            rollDirection += PlayerCamera.Instance.GetCamera().transform.right * PlayerInputManager.Instance.GetHorizontalInput();
            rollDirection.y = 0;
            rollDirection.Normalize();

            Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
            player.transform.rotation = playerRotation;

            player.PlayerAnimatorManager.PlayTargetActionAnimation("Roll", true);
        }
        //  IF WE ARE STATIONARY, PERFORM A BACKSTEP
        else
        {
            player.PlayerAnimatorManager.PlayTargetActionAnimation("BackStep", true);
        }

        player.PlayerNetworkManager.currentStamina.Value -= dodgeStaminaCost;
    }

    public void HandleSprinting()
    {
        if (player.isPerformingAction)
        {
            player.PlayerNetworkManager.isSprinting.Value = false;
        }

        if (player.PlayerNetworkManager.currentStamina.Value <= 0)
        {
            player.PlayerNetworkManager.isSprinting.Value = false;
            return;
        }

        if (moveAmount >= 0.5f)
        {
            player.PlayerNetworkManager.isSprinting.Value = true;
        }
        else
        {
            player.PlayerNetworkManager.isSprinting.Value = false;
        }

        if (player.PlayerNetworkManager.isSprinting.Value)
        {
            player.PlayerNetworkManager.currentStamina.Value -= sprintingStaminaCost * Time.deltaTime;
        }
    }
}
