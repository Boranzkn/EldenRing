using UnityEngine;

public class PlayerStatsManager : CharacterStatsManager
{
    private PlayerManager player;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    protected override void Start()
    {
        base.Start();

        CalculateHealthBasedOnVitalityLevel(player.PlayerNetworkManager.vitality.Value);
        CalculateStaminaBasedOnEnduranceLevel(player.PlayerNetworkManager.endurance.Value);
    }
}
