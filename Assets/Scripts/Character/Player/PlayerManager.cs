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

    protected override void LateUpdate()
    {
        if (!IsOwner)
            return;

        base.LateUpdate();

        PlayerCamera.instance.HandleAllCameraActions();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            PlayerCamera.instance.player = this;
        }
    }
}
