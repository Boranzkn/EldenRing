using UnityEngine;

public class PlayerLocalmotionManager : CharacterLocalmotionManager
{
    private PlayerManager player;

    [Header("Movement")]
    private CharacterController characterController;
    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;
    private float verticalMovement;
    private float horizontalMovement;
    private float moveAmount;
    private float walkingSpeed = 2;
    private float runningSpeed = 5;
    private float sprintingSpeed = 7;
    private float rotationSpeed = 15;
    private int sprintingStaminaCost = 10;

    [Header("Dodge")]
    private Vector3 rollDirection;
    private float dodgeStaminaCost = 20;

    [Header("Jump")]
    private Vector3 jumpDirection;
    private float jumpStaminaCost = 20;
    private float jumpHeight = 4;
    private float jumpForwardSpeed= 5;
    private float freeFallSpeed= 2;


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
            player.CharacterNetworkManager.horizontalMovement.Value = horizontalMovement;
            player.CharacterNetworkManager.verticalMovement.Value = verticalMovement;
            player.CharacterNetworkManager.moveAmount.Value = moveAmount;
        }
        else
        {
            horizontalMovement = player.CharacterNetworkManager.horizontalMovement.Value;
            verticalMovement = player.CharacterNetworkManager.verticalMovement.Value;
            moveAmount = player.CharacterNetworkManager.moveAmount.Value;

            //  IF NOT LOCKED ON, PASS MOVE AMOUNT
            player.PlayerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.PlayerNetworkManager.isSprinting.Value);

            //  IF LOCKED ON, PASS HORIZONTAL AND VERTICAL
        }
    }

    public void HandleAllMovement()
    {
        HandleGroundedMovement();
        HandleRotation();
        HandleJumpingMovement();
        HandleFreeFallMovement();
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

    private void HandleJumpingMovement()
    {
        if (player.isJumping)
        {
            player.GetCharacterController().Move(jumpDirection * jumpForwardSpeed * Time.deltaTime);
        }
    }

    private void HandleFreeFallMovement()
    {
        if (!player.isGrounded)
        {
            Vector3 freeFallDirection;

            freeFallDirection = PlayerCamera.Instance.transform.forward * PlayerInputManager.Instance.GetVerticalInput();
            freeFallDirection += PlayerCamera.Instance.transform.right * PlayerInputManager.Instance.GetHorizontalInput();
            freeFallDirection.y = 0;

            player.GetCharacterController().Move(freeFallDirection * freeFallSpeed * Time.deltaTime);
        }
    }


    private void GetMovementValues()
    {
        verticalMovement = PlayerInputManager.Instance.GetVerticalInput();
        horizontalMovement = PlayerInputManager.Instance.GetHorizontalInput();
        moveAmount = PlayerInputManager.Instance.GetMoveAmount();
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

    public void AttemptToPerformJump()
    {
        if (player.isPerformingAction || player.PlayerNetworkManager.currentStamina.Value <= 0 || player.isJumping || !player.isGrounded) return;

        player.PlayerAnimatorManager.PlayTargetActionAnimation("Jump Start", false);

        player.isJumping = true;

        player.PlayerNetworkManager.currentStamina.Value -= jumpStaminaCost;

        jumpDirection = PlayerCamera.Instance.GetCamera().transform.forward * PlayerInputManager.Instance.GetVerticalInput();
        jumpDirection += PlayerCamera.Instance.GetCamera().transform.right * PlayerInputManager.Instance.GetHorizontalInput();
        jumpDirection.y = 0;

        if (jumpDirection != Vector3.zero)
        {
            if (player.PlayerNetworkManager.isSprinting.Value)
            {
                jumpDirection *= 1;
            }
            else if (PlayerInputManager.Instance.GetMoveAmount() > 0.5f)
            {
                jumpDirection *= 0.5f;
            }
            else if (PlayerInputManager.Instance.GetMoveAmount() <= 0.5f)
            {
                jumpDirection *= 0.25f;
            }
        }
    }

    public void ApplyJumpingVelocity()
    {
        yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);
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
