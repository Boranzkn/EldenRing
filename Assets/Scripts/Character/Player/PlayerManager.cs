using UnityEngine;

public class PlayerManager : CharacterManager
{
    private PlayerLocalmotionManager playerLocalmotionmanager;

    protected override void Awake()
    {
        base.Awake();

        playerLocalmotionmanager = GetComponent<PlayerLocalmotionManager>();
    }

    protected override void Update()
    {
        base.Update();

        if (!IsOwner)
        {
            return;
        }

        playerLocalmotionmanager.HandleAllMovement();
    }
}
