using UnityEngine;

public class PlayerLocalmotionManager : CharacterLocalmotionManager
{
    public float verticalMovement;
    public float horizontalMovement;
    public float moveAmount;

    [SerializeField] private float walkingSpeed = 2;
    [SerializeField] private float runningSpeed = 5;
    [SerializeField] private float rotationSpeed = 15;

    private PlayerManager player;
    private CharacterController characterController;
    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;

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
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount);

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
        verticalMovement = PlayerInputManager.instance.GetVerticalInput();
        horizontalMovement = PlayerInputManager.instance.GetHorizontalInput();
        moveAmount = PlayerInputManager.instance.GetMoveAmount();
    }

    private void HandleGroundedMovement()
    {
        GetMovementValues();

        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection += PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (PlayerInputManager.instance.GetMoveAmount() > 0.5f)
        {
            // MOVE AT A RUNNING SPEED
            characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
        }
        else if (PlayerInputManager.instance.GetMoveAmount() <= 0.5f)
        {
            // MOVE AT A WALKING SPEED
            characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
        }
    }

    private void HandleRotation()
    {
        targetRotationDirection = Vector3.zero;
        targetRotationDirection = PlayerCamera.instance.GetCamera().transform.forward * verticalMovement;
        targetRotationDirection += PlayerCamera.instance.GetCamera().transform.right * horizontalMovement;
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
}
