using UnityEngine;

public class PlayerManager : CharacterManager
{
    PlayerLocalmotionManager playerLocalmotionmanager;

    protected override void Awake()
    {
        base.Awake();

        playerLocalmotionmanager = GetComponent<PlayerLocalmotionManager>();
    }

    protected override void Update()
    {
        base.Update();

        playerLocalmotionmanager.HandleAllMovement();
    }
}
