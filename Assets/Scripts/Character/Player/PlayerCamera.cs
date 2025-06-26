using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    
    [HideInInspector] public PlayerManager player;

    [SerializeField] private Camera cameraObject;

    [Header("CameraSettings")]
    [SerializeField] private Transform cameraPivotTransform;
    private Vector3 cameraVelocity;
    private float leftAndRightLookAngle;
    private float upAndDownLookAngle;
    private float cameraSmoothSpeed = 1;
    private float leftAndRightRotationSpeed = 220;
    private float upAndDownRotationSpeed = 220;
    private float minimumPivot = -30;   //  THE LOWEST POINT YOU ARE ABLE TO LOOK DOWN 
    private float maximumPivot = 60;    //  THE HIGHEST POINT YOU ARE ABLE TO LOOK UP

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
    }

    public void HandleAllCameraActions()
    {
        if (player != null)
        {
            HandleFollowTarget();
            HandleRotation();
        }
    }

    private void HandleFollowTarget()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
        transform.position = targetCameraPosition;
    }

    private void HandleRotation()
    {
        //  IF CAMERA LOCK ON TO A TARGET, FORCE ROTATION TOWARDS TARGET
        //  ELSE ROTATE REGULARLY


        //  NORMAL ROTATIONS
        Vector3 cameraRotation = Vector3.zero;
        Quaternion targetRotation;
        SetLookAngles();
        RotateLeftAndRight();
        RotatePivotUpAndDown();




        void SetLookAngles()
        {
            leftAndRightLookAngle += (PlayerInputManager.instance.GetCameraHorizontalInput() * leftAndRightRotationSpeed) * Time.deltaTime;
            upAndDownLookAngle -= (PlayerInputManager.instance.GetCameraVerticalInput() * upAndDownRotationSpeed) * Time.deltaTime;
            upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);
        }

        void RotateLeftAndRight()
        {
            cameraRotation.y = leftAndRightLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            transform.rotation = targetRotation;
        }

        void RotatePivotUpAndDown()
        {
            cameraRotation = Vector3.zero;
            cameraRotation.x = upAndDownLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            cameraPivotTransform.localRotation = targetRotation;
        }

    }

    public Camera GetCamera()
    {
        return cameraObject;
    }
}
