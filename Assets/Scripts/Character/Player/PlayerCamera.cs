using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    
    [HideInInspector] public PlayerManager player;

    [SerializeField] private Camera cameraObject;
    [SerializeField] private LayerMask collideWithLayers;

    [Header("CameraSettings")]
    [SerializeField] private Transform cameraPivotTransform;
    private Vector3 cameraVelocity;
    private Vector3 cameraObjectPosition;
    private float leftAndRightLookAngle;
    private float upAndDownLookAngle;
    private float cameraSmoothSpeed = 1;
    private float leftAndRightRotationSpeed = 220;
    private float upAndDownRotationSpeed = 220;
    private float minimumPivot = -30;   //  THE LOWEST POINT YOU ARE ABLE TO LOOK DOWN 
    private float maximumPivot = 60;    //  THE HIGHEST POINT YOU ARE ABLE TO LOOK UP
    private float cameraCollisionRadius = 0.2f;
    private float cameraZPosition;
    private float targetCameraZPosition;

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

        cameraZPosition = cameraObject.transform.localPosition.z;
    }

    public void HandleAllCameraActions()
    {
        if (player != null)
        {
            HandleFollowTarget();
            HandleRotation();
            HandleCollisions();
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

    private void HandleCollisions()
    {
        targetCameraZPosition = cameraZPosition;

        Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out RaycastHit hit, Mathf.Abs(targetCameraZPosition), collideWithLayers))
        {
            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
        }

        if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
        {
            targetCameraZPosition = -cameraCollisionRadius;
        }

        cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
        cameraObject.transform.localPosition = cameraObjectPosition;
    }

    public Camera GetCamera()
    {
        return cameraObject;
    }
}
