using UnityEngine;

public class PlayerAnimatorManager : CharacterAnimatorManager
{
    private PlayerManager player;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    private void OnAnimatorMove()
    {
        Vector3 velocity = player.animator.deltaPosition;
        player.GetCharacterController().Move(velocity);
        player.transform.rotation *= player.animator.deltaRotation;
    }
}
