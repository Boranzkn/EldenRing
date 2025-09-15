using UnityEngine;

public class CharacterLocalmotionManager : MonoBehaviour
{
    CharacterManager character;

    [Header("Grounded Chech & Jump")]
    [SerializeField] private LayerMask groundLayer;
    protected Vector3 yVelocity;
    protected float groundedYVelocity = -20;
    protected float fallStartYVelocity = -5;
    protected float inAirTimer = 0;
    protected float gravityForce = -40f;
    protected bool fallingVelocityHasBeenSet = false;
    private float groundCheckSphereRadius = 0.3f;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    protected virtual void Update()
    {
        HandleGroundCheck();

        if (character.isGrounded)
        {
            if (yVelocity.y < 0)
            {
                inAirTimer = 0;
                fallingVelocityHasBeenSet = false;
                yVelocity.y = groundedYVelocity;
            }
        }
        else
        {
            if (!character.isJumping && !fallingVelocityHasBeenSet)
            {
                fallingVelocityHasBeenSet = true;
                yVelocity.y = fallStartYVelocity;
            }

            inAirTimer += Time.deltaTime;

            character.Animator.SetFloat("InAirTimer", inAirTimer);

            yVelocity.y += gravityForce * Time.deltaTime;
        }

        character.GetCharacterController().Move(yVelocity * Time.deltaTime);
    }

    protected void HandleGroundCheck()
    {
        character.isGrounded = Physics.CheckSphere(character.transform.position, groundCheckSphereRadius, groundLayer);
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(character.transform.position, groundCheckSphereRadius);
    }
}
