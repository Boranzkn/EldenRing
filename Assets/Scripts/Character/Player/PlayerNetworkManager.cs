using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkManager : CharacterNetworkManager
{
    private PlayerManager player;

    public NetworkVariable<FixedString64Bytes> characterName = new NetworkVariable<FixedString64Bytes>("Character", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Equipment")]
    public NetworkVariable<int> currentRightHandWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> currentLeftHandWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    public void SetNewMaxHealthValue(int oldVitality, int newVitality)
    {
        maxHealth.Value = player.PlayerStatsManager.CalculateHealthBasedOnVitalityLevel(newVitality);
        PlayerUIManager.Instance.PlayerUIHudManager.SetMaxHealthValue(maxHealth.Value);
        currentHealth.Value = maxHealth.Value;
    }

    public void SetNewMaxStaminaValue(int oldEndurance, int newEndurance)
    {
        maxStamina.Value = player.PlayerStatsManager.CalculateStaminaBasedOnEnduranceLevel(newEndurance);
        PlayerUIManager.Instance.PlayerUIHudManager.SetMaxStaminaValue(maxStamina.Value);
        currentStamina.Value = maxStamina.Value;
    }

    public void OnCurrentRightHandWeaponIDChange(int oldValue, int newValue)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(newValue));
        player.PlayerInventoryManager.currentRightHandWeapon = newWeapon;
        player.PlayerEquipmentManager.LoadRightWeapon();
    }

    public void OnCurrentLeftHandWeaponIDChange(int oldValue, int newValue)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(newValue));
        player.PlayerInventoryManager.currentLeftHandWeapon = newWeapon;
        player.PlayerEquipmentManager.LoadLeftWeapon();
    }
}
