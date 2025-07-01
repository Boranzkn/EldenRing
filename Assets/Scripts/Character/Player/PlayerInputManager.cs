using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;

    public PlayerManager player;

    [SerializeField] private Vector2 movementInput;
    [SerializeField] private Vector2 cameraInput;
    [SerializeField] private bool dodgeInput = false;
    [SerializeField] private bool sprintInput = false;

    private PlayerControls playerControls;
    private float horizontalInput;
    private float verticalInput;
    private float cameraHorizontalInput;
    private float cameraVerticalInput;
    private float moveAmount;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        SceneManager.activeSceneChanged += OnSceneChanged;

        instance.enabled = false;
    }

    private void Update()
    {
        HandleAllInputs();
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        // IF LOADING INTO WORLD SCENE, ENABLE PLAYER CONTROLS
        if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
        {
            instance.enabled = true;
        }
        else
        {
            instance.enabled=false;
        }
    }

    private void HandleAllInputs()
    {
        HandlePlayerMovementInput();
        HandleCameraMovementInput();
        HandleDodgeInput();
        HandleSprinting();
    }

    private void HandlePlayerMovementInput()
    {
        horizontalInput = movementInput.x;
        verticalInput = movementInput.y;

        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        // CLAMP THE VALUES, SO THEY ARE 0, 0.5 OR 1
        if (moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5 && moveAmount <= 1)
        {
            moveAmount = 1;
        }


        if (player == null)
            return;


        //  IF WE ARE NOT LOCKED ON, ONLY USE THE MOVE AMOUNT AND PASS 0 TO HORIZONTAL
        player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);

        //  IF WE ARE LOCKED ON, PASS THE HORIZONTAL MOVEMENT AS WELL
    }

    private void HandleCameraMovementInput()
    {
        cameraHorizontalInput = cameraInput.x;
        cameraVerticalInput = cameraInput.y;
    }

    private void HandleDodgeInput()
    {
        if (dodgeInput)
        {
            dodgeInput = false;

            player.playerLocalmotionmanager.AttemptToPerformDodge();
        }
    }

    private void HandleSprinting()
    {
        if (sprintInput)
        {
            player.playerLocalmotionmanager.HandleSprinting();
        }
        else
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }
    }


    //  GETTER METHODS
    public float GetVerticalInput()
    {
        return verticalInput;
    }

    public float GetHorizontalInput()
    {
        return horizontalInput;
    }

    public float GetCameraHorizontalInput()
    {
        return cameraHorizontalInput;
    }

    public float GetCameraVerticalInput()
    {
        return cameraVerticalInput;
    }

    public float GetMoveAmount()
    {
        return moveAmount;
    }

    // WHEN MINIMIZE OR LOWER THE WINDOW, STOP ADJUSTING INPUTS
    private void OnApplicationFocus(bool focus)
    {
        if (enabled)
        {
            if (focus)
            {
                playerControls.Enable();
            }
            else
            {
                playerControls.Disable();
            }
        }
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerCamera.Movement.performed += i => cameraInput = i.ReadValue<Vector2>();
            //playerControls.PlayerCamera.Mouse.performed += i => cameraInput = i.ReadValue<Vector2>();
            playerControls.PlayerActions.Dodge.performed += i => dodgeInput = true;
            playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
            playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;
        }

        playerControls.Enable();
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }
}
